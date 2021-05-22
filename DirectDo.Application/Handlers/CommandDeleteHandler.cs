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
        private readonly IAlertCommandRepository _repository;
        private readonly IServerMessenger _messenger;

        public CommandDeleteHandler(IAlertCommandRepository repository, IServerMessenger messenger)
        {
            _repository = repository;
            _messenger = messenger;
        }

        public Task Handle(DeleteCommand notification, CancellationToken cancellationToken)
        {
            var cmd = _repository.Find(notification.Id);
            if (cmd != null)
            {
                _repository.RemoveCommand(notification.Id);
                _messenger.SendMessage(notification.Id, "The command deleted");
            }

            return Task.CompletedTask;
        }
    }
}