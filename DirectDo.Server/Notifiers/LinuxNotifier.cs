using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DirectDo.Application;

namespace DirectDo.Server.Notifiers
{
    public class LinuxNotifier : INotify
    {
        private readonly string _audioPath =
            Path.Combine(Directory.GetCurrentDirectory()) + "/Audios/bell.wav";

        public async Task NotifyAsync(string content, int? id)
        {
            if (id != null)
            {
                var audioProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = $"paplay",
                        Arguments = _audioPath,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                    }
                };
                audioProcess.Start();
                await audioProcess.WaitForExitAsync();
            }

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