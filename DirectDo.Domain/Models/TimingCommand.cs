using MediatR;
using System;
using System.Collections.Generic;
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

}