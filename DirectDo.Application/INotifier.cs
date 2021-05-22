using System.Threading.Tasks;

namespace DirectDo.Application
{
    public interface INotifier
    {
        public Task NotifyAsync(string content, bool sound);
    }
}