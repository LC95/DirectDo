using MediatR;
using System;
using System.Text;

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
    public readonly struct TimeIndexer : IComparable<TimeIndexer>, IEquatable<TimeIndexer>
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

        public bool Equals(TimeIndexer other)
        {
            return Id.Equals(other.Id) && AlertTime.Equals(other.AlertTime);
        }

        public override bool Equals(object obj)
        {
            return obj is TimeIndexer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, AlertTime);
        }
    }

    public abstract class TimingCommand : IControlCommand
    {
        public readonly bool IsAlarm;

        public readonly string Message;

        public DateTime AlertTime;

        public TimeSpan RemainTime => GetRemainTime();

        private TimeSpan GetRemainTime()
        {
            var t = AlertTime - DateTime.Now;
            return t < TimeSpan.Zero ? TimeSpan.FromMilliseconds(100) : t;
        }
        
        protected TimingCommand(Guid id, DateTime alertTime, bool isAlarm, string message)
        {
            Id = id;
            AlertTime = alertTime;
            IsAlarm = isAlarm;
            Message = message ?? string.Empty;
        }

        public Guid Id { get; }
        public abstract bool IsComplete { get; }

        public abstract void AfterRun();

        public TimeIndexer Indexer => new(Id, AlertTime);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Id:{Id}---");
            sb.Append($"Message:{Message}---");
            sb.AppendFormat("IsAlarm:{0}---", IsAlarm);
            sb.Append($"IsComplete:{IsComplete}---");
            sb.Append($"AlertTime:{AlertTime}---");
            return sb.ToString();
        }
    }
}