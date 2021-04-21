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
        public readonly int? IsAlarm;

        public readonly string Message;

        public DateTime AlertTime;

        protected TimingCommand(Guid id,DateTime alertTime, int? isAlarm, string message)
        {
            Id = id;
            AlertTime = alertTime;
            IsAlarm = isAlarm;
            Message = message;
        }

        public Guid Id { get; }
        public abstract bool IsComplete { get; }

        public abstract void AfterRun();
    }
}