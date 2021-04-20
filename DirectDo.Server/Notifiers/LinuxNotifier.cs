using DirectDo.Application;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DirectDo.Server
{
    public class LinuxNotifier : INotify
    {
        public async Task NotifyAsync(string content)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = $"notify-send",
                    Arguments = $"DirectDo \"{content}\" -i applications-office -c im",
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            await process.WaitForExitAsync();
        }
    }
}