using System.Threading.Tasks;

namespace DirectDo.Application
{
    public interface INotify
    {
        public Task NotifyAsync(string content);
    }
}