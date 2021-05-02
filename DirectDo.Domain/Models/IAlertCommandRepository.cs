using System;
using System.Collections.Generic;

namespace DirectDo.Domain.Models
{
    public interface IAlertCommandRepository
    {
        public void AddCommand(TimingCommand timingCommand);

        public void RemoveCommand(Guid id);

        public void UpdateCommand(TimingCommand command);

        public TimingCommand? Find(Guid id);

        public IEnumerable<TimingCommand> All();
    }
}