using DirectDo.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DirectDo.Application
{
    public class AlertCommandRepository : IAlertCommandRepository
    {
        private readonly ILogger<AlertCommandRepository> _logger;
        private readonly IMediator _mediator;

        //等待的通知队列
        private readonly Dictionary<Guid, TimingCommand> commands = new();

        public AlertCommandRepository(IMediator mediator, ILogger<AlertCommandRepository> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public void AddCommand(TimingCommand timingCommand)
        {
            commands.Add(timingCommand.Id, timingCommand);
        }

        public void RemoveCommand(Guid id)
        {
            commands.Remove(id);
        }

        public void UpdateCommand(TimingCommand command)
        {
            return;
        }

        public TimingCommand Find(Guid id)
        {
            return commands[id];
        }
    }
}