using System;
using System.Threading.Tasks;
using DirectDo.Application;
using NetMQ;
using NetMQ.Sockets;

namespace DirectDo.Server
{
    public class ServerMessenger : IServerMessenger
    {
        private readonly RouterSocket _router;

        public ServerMessenger(RouterSocket router)
        {
            _router = router;
        }


        public void SendMessage(Guid clientId, string content)
        {
            var messageToClient = new NetMQMessage();
            messageToClient.Append(clientId.ToByteArray());
            messageToClient.AppendEmptyFrame();
            messageToClient.Append(content);

            _router.SendMultipartMessage(messageToClient);
        }

    }
}