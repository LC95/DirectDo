using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DirectDo.Client
{
    internal class Program
    {
        private readonly PushSocket _pushSocket = new(">tcp://127.0.0.1:5556");
        private readonly SubscriberSocket _subscriberSocket = new(">tcp://127.0.0.1:5557");

        private Program()
        {
            _subscriberSocket.SubscribeToAnyTopic();
        }

        private static async Task Main(string[] args)
        {
            await new Program().Run(args);
        }

        private async Task Run(string[] args)
        {
            var subscribeJob = Task.Run(DoOnSubscriberListen);

            _pushSocket.SendFrame(JsonConvert.SerializeObject(args));
            Console.ReadKey();
        }

        private Task DoOnSubscriberListen()
        {
            while (true)
            {
                var title = _subscriberSocket.ReceiveFrameString();
                var message = _subscriberSocket.ReceiveFrameString();
                Console.WriteLine($"{title}:{message}");
                Console.WriteLine("按任意键退出！");
            }
        }

        private static Task SendMessageAsync(string msg)
        {
            var client = new RequestSocket(">tcp://localhost:5556");

            client.SendFrame(msg); // Message
            return Task.CompletedTask;
        }
    }
}