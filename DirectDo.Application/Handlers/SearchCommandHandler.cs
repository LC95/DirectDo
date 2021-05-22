using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Models;
using MediatR;

namespace DirectDo.Application.Handlers
{
    public class SearchCommandHandler : INotificationHandler<SearchCommand>
    {
        private readonly IAlertCommandRepository _repository;
        private readonly IServerMessenger _messenger;

        public SearchCommandHandler(IAlertCommandRepository repository, IServerMessenger messenger)
        {
            _repository = repository;
            _messenger = messenger;
        }

        public Task Handle(SearchCommand notification, CancellationToken cancellationToken)
        {
            var msg = string.Join("\n", _repository.All().Select(t => t.ToString()));
            _messenger.SendMessage(notification.Id, msg);
            return Task.CompletedTask;
        }
    }
}