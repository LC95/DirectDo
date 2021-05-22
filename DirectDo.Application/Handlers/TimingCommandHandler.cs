using DirectDo.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace DirectDo.Application.Handlers
{
    public class TimingCommandHandler : INotificationHandler<TimingAlertCommand>
    {
        private readonly INotifier _notifier;
        private readonly IAlertCommandRepository _alertCommandRepository;
        private readonly ILogger<TimingCommandHandler> _logger;
        public TimingCommandHandler(INotifier notifier, ILogger<TimingCommandHandler> logger, IAlertCommandRepository alertCommandRepository)
        {
            _notifier = notifier;
            _logger = logger;
            _alertCommandRepository = alertCommandRepository;
        }

        public async Task Handle(TimingAlertCommand alertCommand, CancellationToken cancellationToken)
        {
            await _notifier.NotifyAsync(alertCommand.Message, alertCommand.IsAlarm);
            _alertCommandRepository.RemoveCommand(alertCommand.Id);
            alertCommand.AfterRun();
            if (!alertCommand.IsComplete)
            {
                _alertCommandRepository.AddCommand(alertCommand);
            }
            _logger.LogInformation($"ID:{alertCommand.Id} is handled and its complete status is {alertCommand.IsComplete}");
        }
    }
}