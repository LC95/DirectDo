using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DirectDo.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DirectDo.Domain
{
    /// <summary>
    /// TODO:remove sync problem
    /// </summary>
    public class NockNock : IClock
    {
        record Operation
        {
            public TimeIndexer Indexer { get; }
            public bool Type { get; }

            public Operation(TimeIndexer indexer, bool type)
            {
                Indexer = indexer;
                Type = type;
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly Channel<Operation> _alertCreateChannel;
        private readonly ManualResetEventSlim _hasCanceled;
        private readonly SortedSet<TimeIndexer> _sortedIndexers;
        private readonly IMediator _mediator;
        private readonly ILogger<NockNock> _logger;
        private Task _runListen;
        private readonly IAlertCommandRepository _alertCommandRepository;

        public NockNock(IAlertCommandRepository alertCommandRepository, IMediator mediator, ILogger<NockNock> logger)
        {
            _alertCommandRepository = alertCommandRepository;
            _mediator = mediator;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _alertCreateChannel = Channel.CreateUnbounded<Operation>();
            _hasCanceled = new ManualResetEventSlim(false);
            _sortedIndexers = new SortedSet<TimeIndexer>();
            _runListen = AddRemoveListenJob();
        }

        /// <summary>
        /// though may multiple threads will run this code, it will be sync by channel
        /// </summary>
        /// <param name="indexer"></param>
        public ValueTask AddAlertTimeAsync(TimeIndexer indexer)
        {
            return _alertCreateChannel.Writer.WriteAsync(new Operation(indexer, true));
        }

        public ValueTask RemoveAsync(TimeIndexer indexer)
        {
            return _alertCreateChannel.Writer.WriteAsync(new Operation(indexer, false));
        }

        /// <summary>
        /// This loop method will executed in only one thread
        /// </summary>
        private async Task AddRemoveListenJob()
        {
            while (true)
            {
                var op = await _alertCreateChannel.Reader.ReadAsync();
                var indexerToOperate = op.Indexer;

                if (op.Type) //写操作
                {
                    _sortedIndexers.Add(indexerToOperate);
                    var min = _sortedIndexers.Min;
                    //在没有定时或者更近的定时来临时取消当前的等待
                    if (indexerToOperate.CompareTo(min) <= 0)
                    {
                        _cancellationTokenSource.Cancel();
                        //等待BeginWaitAsync取消操作完成
                        _hasCanceled.Wait();
                        _logger.LogInformation($"A Cancellation has finished for a new nock {indexerToOperate}");
                    }
                }
                else
                {
                    var min = _sortedIndexers.Min;
                    _sortedIndexers.Remove(indexerToOperate);
                    if (indexerToOperate.Equals(min))
                    {
                        //如果当前等待的正是要取消运行的时刻。要取消当前的运行，执行一个新的运行。
                        _cancellationTokenSource.Cancel();
                        _hasCanceled.Wait();
                        _logger.LogInformation($"A Cancellation has finished for a remove {indexerToOperate}");
                    }
                }
            }
        }

        private async Task WaitWhenCommandNeedToExeAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var waitTime = GetWaitTimeSpan();
            if (waitTime != TimeSpan.Zero)
            {
                await Task.Delay(waitTime, _cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// This loop method will executed in only one thread
        /// </summary>
        public async Task BeginWaitAsync()
        {
            while (true)
            {
                try
                {
                    await WaitWhenCommandNeedToExeAsync();
                    var recentTimeIndexer = _sortedIndexers.FirstOrDefault();
                    _sortedIndexers.Remove(recentTimeIndexer);

                    var cmd = _alertCommandRepository.Find(recentTimeIndexer.Id);
                    if (cmd != null)
                    {
                        await _mediator.Publish(cmd);
                        if (cmd.IsComplete)
                        {
                            _alertCommandRepository.RemoveCommand(cmd.Id);
                        }
                        else
                        {
                            _alertCommandRepository.UpdateCommand(cmd);
                            await AddAlertTimeAsync(cmd.Indexer); //重新发布一个定时标记
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //有距离现在最近的请求来临时，取消当前的等待行为。
                    //并通知取消操作线程已经完成。
                    _hasCanceled.Set();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }

        /// <summary>
        /// 获取应当等待的时间
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetWaitTimeSpan()
        {
            //如果当前没有请求，就等待无限长的时间，未来有请求了，就会打断这次等待
            if (_sortedIndexers.Count == 0)
            {
                return TimeSpan.FromMilliseconds(-1);
            }

            //计算时间
            var timeSpan = _sortedIndexers.Min.AlertTime - DateTime.Now;
            //判断正负，将负值变成0.代表着直接执行
            return timeSpan > TimeSpan.Zero ? timeSpan : TimeSpan.Zero;
        }
    }
}