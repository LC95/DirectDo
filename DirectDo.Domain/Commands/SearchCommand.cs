using System;
using DirectDo.Domain.Models;

namespace DirectDo.Domain.Commands
{
    public class SearchCommand : IControlCommand
    {
        public Guid Id => Guid.NewGuid();
    }
}