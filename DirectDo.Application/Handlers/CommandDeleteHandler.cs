using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Models;
using MediatR;

namespace DirectDo.Application.Handlers
{
    public class CommandDeleteHandler : INotificationHandler<DeleteCommand>
    {
        private readonly IClock _clock;
        private readonly IAlertCommandRepository _repository;
        private readonly IServerMessenger _messenger;

        public CommandDeleteHandler(IClock clock, IAlertCommandRepository repository, IServerMessenger messenger)
        {
            _clock = clock;
            _repository = repository;
            _messenger = messenger;
        }

        public async Task Handle(DeleteCommand notification, CancellationToken cancellationToken)
        {
            var cmd = _repository.Find(notification.Id);
            if (cmd != null)
            {
                await _clock.RemoveAsync(new TimeIndexer(notification.Id, cmd.AlertTime));
                _repository.RemoveCommand(notification.Id);
                _messenger.SendMessage(notification.Id, "The command deleted");
            }
        }
    }
}