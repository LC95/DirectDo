using DirectDo.Domain.Events;
using DirectDo.Domain.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain;

namespace DirectDo.Application.Handlers
{
    public class TimingCreatedHandler : INotificationHandler<TimingCreatedNotification>
    {
        private readonly IAlertCommandRepository _alertCommandRepository;
        private readonly IClock _clock;
        private readonly IServerMessenger _messenger;

        public TimingCreatedHandler(IAlertCommandRepository alertCommandRepository, IClock clock, IServerMessenger messenger)
        {
            _alertCommandRepository = alertCommandRepository;
            _clock = clock;
            _messenger = messenger;
        }

        public async Task Handle(TimingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _clock.AddAlertTimeAsync(notification.AlertCommand.Indexer);
            _alertCommandRepository.AddCommand(notification.AlertCommand);
            _messenger.SendMessage(notification.Id, "Your Commanded Has Been Handled");
        }
    }
}