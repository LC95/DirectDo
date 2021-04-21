using DirectDo.Application;
# if WINDOWS10_0_19041_0
using Microsoft.Toolkit.Uwp.Notifications;
# endif
using System;
using System.IO;
using System.Threading.Tasks;

namespace DirectDo.Server.Notifiers
{
    public class WindowsNotifier : INotify
    {
        string p = Path.GetFullPath("Images/bell.png");
        string h = Path.GetFullPath("Images/hero_image.jpeg");

        public Task NotifyAsync(string content, int? alarm)
        {
#if WINDOWS10_0_19041_0
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddAppLogoOverride(new Uri(p), ToastGenericAppLogoCrop.Default)
            .AddHeroImage(new Uri(h))
            .AddAttributionText("· Adam Ma ·")
            .AddText("通知：")
            .AddCustomTimeStamp(DateTime.Now)
            .AddText(content)
            .Show(); // 
# endif
            return Task.CompletedTask;
        }
    }
}