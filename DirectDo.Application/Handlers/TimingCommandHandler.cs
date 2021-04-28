using DirectDo.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DirectDo.Application.Handlers
{
    public class TimingCommandHandler : INotificationHandler<TimingAlertCommand>
    {
        private readonly INotify _notify;
        private readonly ILogger<TimingCommandHandler> _logger;
        public TimingCommandHandler(INotify notify, ILogger<TimingCommandHandler> logger)
        {
            _notify = notify;
            _logger = logger;
        }

        public async Task Handle(TimingAlertCommand alertCommand, CancellationToken cancellationToken)
        {
            await _notify.NotifyAsync(alertCommand.Message, alertCommand.IsAlarm);
            alertCommand.AfterRun();
           _logger.LogInformation($"ID:{alertCommand.Id} is handled and its complete status is {alertCommand.IsComplete}");
        }
    }
}