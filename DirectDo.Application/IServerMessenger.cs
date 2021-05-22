using System;
using System.Threading.Tasks;

namespace DirectDo.Application
{
    public interface IServerMessenger
    {
        public void SendMessage(Guid clientId, string content);
    }
}