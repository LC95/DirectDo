using DirectDo.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Application.Handlers
{
    public class TimingCommandHandler : INotificationHandler<TimingAlertCommand>
    {
        private readonly INotify _notify;

        public TimingCommandHandler(INotify notify)
        {
            _notify = notify;
        }

        public async Task Handle(TimingAlertCommand alertCommand, CancellationToken cancellationToken)
        {
            await _notify.NotifyAsync(alertCommand.Message, alertCommand.IsAlarm);
            alertCommand.AfterRun();
            Console.WriteLine($"ID:{alertCommand.Id} is handled and its complete status is {alertCommand.IsComplete}");
        }
    }
}