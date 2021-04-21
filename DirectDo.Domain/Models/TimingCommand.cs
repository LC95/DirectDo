using MediatR;
using System;

namespace DirectDo.Domain.Models
{
    public interface IControlCommand : INotification
    {
        public Guid Id { get; }
    }

    public abstract class TimingCommand : IControlCommand
    {
        public readonly bool IsAlarm;

        public readonly string Message;

        public DateTime AlertTime;

        protected TimingCommand(DateTime alertTime, bool isAlarm, string message)
        {
            Id = Guid.NewGuid();
            AlertTime = alertTime;
            IsAlarm = isAlarm;
            Message = message;
        }

        public Guid Id { get; };
        public abstract bool IsComplete { get; }

        public abstract void AfterRun();
    }
}