using MediatR;
using System;

namespace DirectDo.Domain.Models
{
    public interface IControlCommand : INotification
    {
        public Guid Id { get; }
    }

    /// <summary>
    /// 定时标记
    /// 两部分构成，一部分为定时命令的ID，一部分为定时的时刻
    /// </summary>
    public readonly struct TimeIndexer : IComparable<TimeIndexer>
    {
        public TimeIndexer(Guid id, DateTime alertTime)
        {
            Id = id;
            AlertTime = alertTime;
        }

        /// <summary>
        /// 定时标记又
        /// </summary>
        public Guid Id { get; }

        public DateTime AlertTime { get; }

        public int CompareTo(TimeIndexer other)
        {
            var alertTimeComparison = AlertTime.CompareTo(other.AlertTime);
            if (alertTimeComparison != 0) return alertTimeComparison;
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return $"Id : {Id} & AlertTime : {AlertTime}";
        }
    }

    public abstract class TimingCommand : IControlCommand
    {
        public readonly bool IsAlarm;

        public readonly string Message;

        public DateTime AlertTime;

        protected TimingCommand(Guid id, DateTime alertTime, bool isAlarm, string message)
        {
            Id = id;
            AlertTime = alertTime;
            IsAlarm = isAlarm;
            Message = message;
        }

        public Guid Id { get; }
        public abstract bool IsComplete { get; }

        public abstract void AfterRun();

        public TimeIndexer Indexer => new(Id, AlertTime);
    }
}