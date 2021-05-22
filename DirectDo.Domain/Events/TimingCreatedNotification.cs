using System;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Models;

namespace DirectDo.Domain.Events
{
    public class TimingCreatedNotification : IControlCommand
    {
        public Guid Id { get; }
        public TimingCreatedNotification(TimingCommand command)
        {
            Id = command.Id;
            AlertCommand = command;
        }

        public TimingCommand AlertCommand { get; }
    }
}