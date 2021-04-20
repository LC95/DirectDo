using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DirectDo.Application;
using DirectDo.Domain;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Events;
using DirectDo.Domain.Models;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace DirectDo.Service.Workers
{
    public class CommandReceiverWorker : BackgroundService
    {
        private readonly ILogger<CommandReceiverWorker> _logger;
        private readonly IMediator _mediator;
        private readonly PublisherSocket _publisher = new("@tcp://127.0.0.1:5557");
        private readonly PullSocket _pullSocket = new("@tcp://127.0.0.1:5556");

        public CommandReceiverWorker(ILogger<CommandReceiverWorker> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ListenerTask();
        }

        private async Task ListenerTask()
        {
            _logger.LogInformation("Command Receiver Worker Started!");
            while (true)
            {
                var paramMsg = _pullSocket.ReceiveFrameString();
                _publisher.SendMoreFrame("Received").SendFrame($"Your command received:{paramMsg}");
                _logger.LogInformation("From Pusher : {0}", paramMsg);

                var cmd = BuildCommand(paramMsg);
                if (cmd != null) await _mediator.Publish(cmd);
            }
        }

        private IControlCommand BuildCommand(string param)
        {
            var args = JsonConvert.DeserializeObject<string[]>(param);
            var options = Parser.Default.ParseArguments<Options>(args);
            IControlCommand command = null;
            options.WithParsed(o => { command = BuildCommand(o); })
                .WithNotParsed(e =>
                    {
                        _publisher.SendMoreFrame("Error").SendFrame($"Your command error:{e.First().Tag}");
                    }
                );
            return command;
        }

        private IControlCommand BuildCommand(Options options)
        {
            DateTime? at = null;
            TimeSpan? after = null;
            var isAlarm = options.AlarmNumber != null;
            var maintainTimes = options.MaintainTimes;

            if (!string.IsNullOrEmpty(options.At)) at = DateTime.Parse(options.At);

            if (!string.IsNullOrEmpty(options.After)) after = Utils.ParsePeriod(options.After);

            if (at != null)
            {
                var cmd = new AtTimingCommand(at.Value, isAlarm, options.Message);
                return new TimingCreatedNotification(cmd);
            }

            if (after != null)
            {
                var next = DateTime.Now.Add(after.Value);
                var cmd = new PeriodTimingAlertCommand(next, new Times(maintainTimes), after.Value, isAlarm,
                    options.Message);
                return new TimingCreatedNotification(cmd);
            }

            if (string.IsNullOrEmpty(options.Search))
                return new SearchCommand(options.Search);
            if (string.IsNullOrWhiteSpace(options.Delete)) return new DeleteCommand(options.Delete);

            throw new ArgumentException("参数无法转换");
        }
    }
}