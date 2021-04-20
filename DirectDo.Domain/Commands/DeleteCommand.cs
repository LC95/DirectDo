using DirectDo.Domain.Models;

namespace DirectDo.Domain.Commands
{
    public class DeleteCommand : IControlCommand
    {
        public DeleteCommand(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}