using System;
using MediatR;

namespace DirectDo.Domain.Models
{
    public interface IControlCommand : INotification
    {
    }

    public abstract class TimingCommand : IControlCommand
    {
        public readonly bool IsAlarm;

        public readonly string Message;

        public DateTime AlertTime;

        protected TimingCommand(DateTime alertTime, bool isAlarm, string message)
        {
            AlertTime = alertTime;
            IsAlarm = isAlarm;
            Message = message;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public abstract bool IsComplete { get; }

        public abstract void AfterRun();
    }
}