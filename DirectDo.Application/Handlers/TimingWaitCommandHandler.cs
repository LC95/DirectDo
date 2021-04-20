using DirectDo.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Application.Handlers
{
    public class TimingWaitCommandHandler : INotificationHandler<AtTimingCommand>
    {
        private readonly INotify _notify;

        public TimingWaitCommandHandler(INotify notify)
        {
            _notify = notify;
        }

        public async Task Handle(AtTimingCommand command, CancellationToken cancellationToken)
        {
            await _notify.NotifyAsync(command.Message);
            command.AfterRun();
            Console.WriteLine($"ID:{command.Id} is handled and its complete status is {command.IsComplete}");
        }
    }
}