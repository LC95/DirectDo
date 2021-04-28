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

        //等待的通知队列
        private readonly Dictionary<Guid, TimingCommand> _commands = new();

        public AlertCommandRepository(ILogger<AlertCommandRepository> logger)
        {
            _logger = logger;
        }

        public void AddCommand(TimingCommand timingCommand)
        {
            _commands.Add(timingCommand.Id, timingCommand);
        }

        public void RemoveCommand(Guid id)
        {
            _commands.Remove(id);
        }

        public void UpdateCommand(TimingCommand command)
        {
            return;
        }

        public TimingCommand Find(Guid id)
        {
            return _commands[id];
        }

        public IEnumerable<TimingCommand> All()
        {
            return _commands.Values;
        }
    }
}