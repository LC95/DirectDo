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
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Server.Workers
{
    public class CommandReceiverWorker : BackgroundService
    {
        private readonly ILogger<CommandReceiverWorker> _logger;
        private readonly IMediator _mediator;
        private readonly NetMQRuntime _runtime;
        private readonly RouterSocket _router;

        public CommandReceiverWorker(ILogger<CommandReceiverWorker> logger,
            IMediator mediator)
        {
            _runtime = new NetMQRuntime();
            _router = new("@tcp://127.0.0.1:5556");
            _logger = logger;
            _mediator = mediator;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _runtime.Run(stoppingToken, ListenerTaskAsync());
            return Task.CompletedTask;
        }

        private async Task ListenerTaskAsync()
        {
            _logger.LogInformation("Command Receiver Worker Started!");
            while (true)
            {
                var paramMsg = await _router.ReceiveMultipartMessageAsync();
                var cmdParam = paramMsg[2].ConvertToString(Encoding.UTF8);
                _logger.LogInformation("From Pusher : {0}", cmdParam);

                try
                {
                    var cmd = BuildCommand(cmdParam);
                    _router.SendMultipartMessage(BuildSendBackMessage(paramMsg, $"Server Received Your Command"));

                    if (cmd != null)
                        await _mediator.Publish(cmd);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    _router.SendMultipartMessage(BuildSendBackMessage(paramMsg, e.ToString()));
                }
            }
        }

        static NetMQMessage BuildSendBackMessage(NetMQMessage origin, string msg)
        {
            var messageToClient = new NetMQMessage();
            messageToClient.Append(origin[0]);
            messageToClient.AppendEmptyFrame();
            messageToClient.Append(msg);
            return messageToClient;
        }

        private IControlCommand BuildCommand(string param)
        {
            var options = JsonConvert.DeserializeObject<AddOptions>(param);

            var command = Utils.BuildCommand(options);
            return command;
        }

        public override void Dispose()
        {
            try
            {
                _router.Dispose();
                _runtime.Dispose();
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