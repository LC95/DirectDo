
/* 项目“DirectDo.Server (net5.0-windows10.0.19041.0)”的未合并的更改
在此之前:
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
在此之后:
using CommandLine;
*/
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

/* 项目“DirectDo.Server (net5.0-windows10.0.19041.0)”的未合并的更改
在此之前:
using Newtonsoft.Json;
在此之后:
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
*/
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Server.Workers
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

                try
                {
                    var cmd = BuildCommand(paramMsg);
                    if (cmd != null) await _mediator.Publish(cmd);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    _publisher.SendMoreFrame("Error").SendFrame(e.ToString());
                }

            }
        }

        private IControlCommand BuildCommand(string param)
        {
            var args = JsonConvert.DeserializeObject<string[]>(param);
            var options = OptionsParse(args);

            IControlCommand command = BuildCommand(options);
            return command;
        }

        private Options OptionsParse(string[] args)
        {
            var option = new Options();
            foreach (var arg in args)
            {
                if (arg.StartsWith('@'))
                {
                    option.At = arg.Substring(1);
                }

                if (arg.StartsWith('>'))
                {
                    option.After = arg.Substring(1);
                }

                if (arg.StartsWith('!'))
                {
                    if (arg.Length == 1)
                    {
                        option.AlarmNumber = null;
                    }
                    else
                    {
                        option.AlarmNumber = int.Parse(arg.Substring(1));
                    }
                }

                if (arg.StartsWith('<'))
                {
                    option.MaintainTimes = int.Parse(arg.Substring(1));
                }

                if (arg.StartsWith('&'))
                {
                    option.Message = arg.Substring(1);
                }
            }
            return option;
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

        public override void Dispose()
        {
            try
            {
                _publisher.Dispose();
                _pullSocket.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}