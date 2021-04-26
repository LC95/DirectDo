using DirectDo.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DirectDo.Application
{
    public class AlertService : IAlertService
    {
        private static readonly TimeSpan INTERVAL_TIME_SPAN = TimeSpan.FromSeconds(1);
        private readonly ILogger<AlertService> _logger;
        private readonly IMediator _mediator;

        //等待的通知队列
        private readonly LinkedList<TimingCommand> _waitList = new();

        public AlertService(IMediator mediator, ILogger<AlertService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public void Add(TimingCommand timingCommand)
        {
            var af = _waitList.FirstOrDefault(t => t.AlertTime >= timingCommand.AlertTime);
            if (af == null)
                _waitList.AddLast(timingCommand);
            else
                _waitList.AddBefore(_waitList.Find(af), timingCommand);
        }

        /// <summary>
        ///     定时检测该通知的内容进行通知
        ///     目前定时时间为<see cref="INTERVAL_TIME_SPAN" />
        /// </summary>
        /// <param name="stoppingToken"></param>
        public async Task RunAlertAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (_waitList.First != null && _waitList.First.Value.AlertTime <= DateTime.Now)
                {
                    try
                    {
                        var first = _waitList.First;
                        var cmd = first.Value;
                        await _mediator.Publish(cmd, stoppingToken);
                        _waitList.RemoveFirst();
                        if (!cmd.IsComplete) Add(cmd);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.ToString());
                    }


                    await Task.Delay(INTERVAL_TIME_SPAN, stoppingToken);
                }

                await Task.Delay(INTERVAL_TIME_SPAN, stoppingToken);
            }
        }

        public IEnumerable<TimingCommand> Search()
        {
            return _waitList;
        }
    }
}