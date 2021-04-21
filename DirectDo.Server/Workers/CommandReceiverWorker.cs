
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
                var (paramMsg, _) = await _pullSocket.ReceiveFrameStringAsync();
                _logger.LogInformation("From Pusher : {0}", paramMsg);

                try
                {
                    var cmd = BuildCommand(paramMsg);
                    _publisher.SendMoreFrame("Received").SendFrame($"Your command Id {cmd.Id} received :{paramMsg}");

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

            IControlCommand command = Utils.BuildCommand(args);
            return command;
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