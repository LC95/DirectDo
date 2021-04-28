using System;
using DirectDo.Domain.Models;

namespace DirectDo.Domain.Commands
{
    public class DeleteCommand : IControlCommand
    {
        public DeleteCommand(Guid id)
        {
            Id = id;
        }


        public Guid Id { get; }
    }
}