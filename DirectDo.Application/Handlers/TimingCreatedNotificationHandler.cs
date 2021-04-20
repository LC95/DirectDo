using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain.Events;
using DirectDo.Domain.Models;
using MediatR;

namespace DirectDo.Application.Handlers
{
    public class TimingCreatedNotificationHandler : INotificationHandler<TimingCreatedNotification>
    {
        private readonly IAlertService _alertService;

        public TimingCreatedNotificationHandler(IAlertService alertService)
        {
            _alertService = alertService;
        }

        public Task Handle(TimingCreatedNotification notification, CancellationToken cancellationToken)
        {
            _alertService.Add(notification.AlertCommand);
            return Task.CompletedTask;
        }
    }
}