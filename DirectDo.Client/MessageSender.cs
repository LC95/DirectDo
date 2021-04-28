using System;
using System.CommandLine;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Application;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace DirectDo.Client
{
    public class MessageSender
    {
        private readonly DealerSocket _dealerSocket;
        private readonly NetMQRuntime _runtime;

        public MessageSender()
        {
            _runtime = new NetMQRuntime();
            _dealerSocket = new(">tcp://127.0.0.1:5556");
            _dealerSocket.Options.Identity = Guid.NewGuid().ToByteArray();
        }

        public Task SendMessageAsync(string[] args)
        {
            _runtime.Run(WrapAsync(args));
            return Task.CompletedTask;
        }

        private Task WrapAsync(string[] args)
        {
            //这里会调用下面的方法
            var cmd = Utils.BuildRootCommand((o) =>
            {
                return ConvertOptionsAndDoNextAsync(o);
            });
            return cmd.InvokeAsync(args);
        }

        private Task ConvertOptionsAndDoNextAsync(AddOptions options)
        {
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(3000));
            var msg = CreateMessage(options);
            return EstablishingConnectionAsync(msg, source.Token);
        }

        private static NetMQMessage CreateMessage(AddOptions options)
        {
            options.ReqId = Guid.NewGuid();

            var msgToSend = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(options));

            var netMessage = new NetMQMessage();
            netMessage.AppendEmptyFrame();
            netMessage.Append(msgToSend);

            return netMessage;
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