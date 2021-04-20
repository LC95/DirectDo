using DirectDo.Domain.Models;

namespace DirectDo.Domain.Events
{
    public class TimingCreatedNotification : IControlCommand
    {
        public TimingCreatedNotification(TimingCommand command)
        {
            AlertCommand = command;
        }

        public TimingCommand AlertCommand { get; }
    }
}