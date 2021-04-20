using System;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain.Commands;
using MediatR;

namespace DirectDo.Application.Handlers
{
    public class RepeatTimingCommandHandler : INotificationHandler<PeriodTimingAlertCommand>
    {
        private readonly INotify _notify;

        public RepeatTimingCommandHandler(INotify notify)
        {
            _notify = notify;
        }

        public async Task Handle(PeriodTimingAlertCommand alertCommand, CancellationToken cancellationToken)
        {
            await _notify.NotifyAsync(alertCommand.Message);
            alertCommand.AfterRun();
            Console.WriteLine($"ID:{alertCommand.Id} is handled and its complete status is {alertCommand.IsComplete}");
        }
    }
}