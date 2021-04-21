using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Client
{
    internal class Program
    {
        private readonly DealerSocket _dealerSocket;
        private readonly NetMQRuntime _runtime;

        private Program()
        {
            _runtime = new NetMQRuntime();
            _dealerSocket = new(">tcp://127.0.0.1:5556");
            _dealerSocket.Options.Identity = Guid.NewGuid().ToByteArray();
        }

        private static void Main(string[] args)
        {
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(9000));
            var p = new Program();
            p.Run(CreateMessage(args), source.Token);
        }

        private static NetMQMessage CreateMessage(string[] args)
        {
            var cmdId = Guid.NewGuid();
            var argsAll = new List<string>(args) {$"#{cmdId}"};
            var msgToSend = JsonConvert.SerializeObject(argsAll);

            var netMessage = new NetMQMessage();
            netMessage.AppendEmptyFrame();
            netMessage.Append(msgToSend);

            return netMessage;
        }

        private void Run(NetMQMessage message, CancellationToken token)
        {
            _runtime.Run(EstablishingConnectionAsync(message, token));
        }

        private async Task EstablishingConnectionAsync(NetMQMessage message, CancellationToken token)
        {
            try
            {
                _dealerSocket.SendMultipartMessage(message);

                var msg = await _dealerSocket.ReceiveMultipartMessageAsync(cancellationToken: token);

                Console.WriteLine($"From Server : {msg[1].ConvertToString()}");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("服务器无响应");
            }
        }
    }
}