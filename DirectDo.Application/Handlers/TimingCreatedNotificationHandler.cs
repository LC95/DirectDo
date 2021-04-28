using DirectDo.Domain.Events;
using DirectDo.Domain.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain;

namespace DirectDo.Application.Handlers
{
    public class TimingCreatedNotificationHandler : INotificationHandler<TimingCreatedNotification>
    {
        private readonly IAlertCommandRepository _alertCommandRepository;
        private readonly IClock _clock;

        public TimingCreatedNotificationHandler(IAlertCommandRepository alertCommandRepository, IClock clock)
        {
            _alertCommandRepository = alertCommandRepository;
            _clock = clock;
        }

        public async Task Handle(TimingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _clock.SetNewAlertTimeAsync(notification.AlertCommand.Indexer);
            _alertCommandRepository.AddCommand(notification.AlertCommand);
        }
    }
}