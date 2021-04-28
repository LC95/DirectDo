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
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Channel<TimeIndexer> _channel;
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
            _channel = Channel.CreateUnbounded<TimeIndexer>();
            _hasCanceled = new ManualResetEventSlim(false);
            _sortedIndexers = new SortedSet<TimeIndexer>();
            _runListen = ListenJob();
        }

        /// <summary>
        /// though may multiple threads will run this code, it will be sync by channel
        /// </summary>
        /// <param name="indexer"></param>
        public ValueTask SetNewAlertTimeAsync(TimeIndexer indexer)
        {
            return _channel.Writer.WriteAsync(indexer);
        }

        public void Remove(Guid notificationId)
        {
            var d = _sortedIndexers.FirstOrDefault(t => t.Id == notificationId);
            _sortedIndexers.Remove(d);
        }

        /// <summary>
        /// This loop method will executed in only one thread
        /// </summary>
        private async Task ListenJob()
        {
            while (true)
            {
                var timeIndexer = await _channel.Reader.ReadAsync();
                _sortedIndexers.Add(timeIndexer);
                var min = _sortedIndexers.Min.AlertTime;
                //在没有定时或者更近的定时来临时取消当前的等待
                if (timeIndexer.AlertTime <= min)
                {
                    _cancellationTokenSource.Cancel();
                    _hasCanceled.Wait();
                    _logger.LogInformation($"A Cancellation has finished for a new nock {timeIndexer}");
                }
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
                    _cancellationTokenSource = new CancellationTokenSource();
                    var waitTime = GetWaitTimeSpan();
                    if (waitTime != TimeSpan.Zero)
                    {
                        await Task.Delay(waitTime, _cancellationTokenSource.Token);
                    }

                    var recentTimeIndexer = _sortedIndexers.FirstOrDefault();
                    _sortedIndexers.Remove(recentTimeIndexer);

                    var cmd = _alertCommandRepository.Find(recentTimeIndexer.Id);
                    //执行通知命令
                    await _mediator.Publish(cmd);
                    if (cmd.IsComplete)
                    {
                        _alertCommandRepository.RemoveCommand(cmd.Id);
                    }
                    else
                    {
                        _alertCommandRepository.UpdateCommand(cmd);
                        await SetNewAlertTimeAsync(cmd.Indexer); //重新发布一个定时标记
                    }
                }
                catch (OperationCanceledException)
                {
                    _hasCanceled.Set();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }

        private TimeSpan GetWaitTimeSpan()
        {
            if (_sortedIndexers.Count == 0)
            {
                return TimeSpan.FromMilliseconds(-1);
            }

            var timeSpan = _sortedIndexers.Min.AlertTime - DateTime.Now;
            return timeSpan > TimeSpan.Zero ? timeSpan : TimeSpan.Zero;
        }
    }
}