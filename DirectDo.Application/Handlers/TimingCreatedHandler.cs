using DirectDo.Domain.Events;
using DirectDo.Domain.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Application.Handlers
{
    public class TimingCreatedHandler : INotificationHandler<TimingCreatedNotification>
    {
        private readonly IAlertCommandRepository _alertCommandRepository;
        private readonly IServerMessenger _messenger;

        public TimingCreatedHandler(IAlertCommandRepository alertCommandRepository, IServerMessenger messenger)
        {
            _alertCommandRepository = alertCommandRepository;
            _messenger = messenger;
        }

        public Task Handle(TimingCreatedNotification notification, CancellationToken cancellationToken)
        {
            _alertCommandRepository.AddCommand(notification.AlertCommand);
            _messenger.SendMessage(notification.Id, "Your Commanded Has Been Handled");
            return Task.CompletedTask;
        }
    }
}