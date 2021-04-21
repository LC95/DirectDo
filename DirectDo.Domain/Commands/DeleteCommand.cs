using System;
using DirectDo.Domain.Models;

namespace DirectDo.Domain.Commands
{
    public class DeleteCommand : IControlCommand
    {
        public DeleteCommand(string id)
        {
            TimingId = id;
        }

        public string TimingId { get; }

        public Guid Id => Guid.NewGuid();
    }
}