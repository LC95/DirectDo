using System.Threading.Tasks;

namespace DirectDo.Application.Utils
{
    public interface INotify
    {
        public Task NotifyAsync(string content);
    }
}