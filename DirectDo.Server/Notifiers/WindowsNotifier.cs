using DirectDo.Application;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DirectDo.Server.Notifiers
{
    public class WindowsNotifier : INotifier
    {
        private readonly Uri _p = new(Path.GetFullPath("Images/bell.png"));
        private readonly Uri _h = new(Path.GetFullPath("Images/hero_image.jpeg"));
        private readonly Uri _a = new(Path.GetFullPath("Audios/bell.wav"));
        public Task NotifyAsync(string content, bool alarm)
        {
            var builder = new ToastContentBuilder()
                // .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 8378)
                .AddAppLogoOverride(_p, ToastGenericAppLogoCrop.Default)
                .AddHeroImage(_h)
                .AddAttributionText("By Adam Ma")
                .AddText("通知：")
                .AddCustomTimeStamp(DateTime.Now)
                .AddText(content);
            if (alarm)
            {
                builder.AddAudio(_a, false, false);
            }
#if WINDOWS10_0_19041_0
            builder.Show(toast=>
            {
                toast.ExpirationTime = DateTime.Now.AddMilliseconds(1000);
            });
# endif
            return Task.CompletedTask;
        }
    }
}