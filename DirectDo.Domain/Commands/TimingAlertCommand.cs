using DirectDo.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectDo.Domain.Commands
{
    /// <summary>
    ///     次数
    /// </summary>
    public struct Times
    {
        private const int REPEAT_PERMANENTLY = -1;
        private int _times;

        public Times(int times = REPEAT_PERMANENTLY)
        {
            _times = times;
        }

        public void Reduce()
        {
            if (_times != REPEAT_PERMANENTLY && _times != 0) _times--;
        }

        public bool IsOver => _times == 0;

        public int Value => _times;
    }

    /// <summary>
    ///     重复指令
    /// </summary>
    public class TimingCommand : IControlCommand
    {
        public readonly bool IsAlarm;

        public readonly string Message;

        private readonly Queue<DateTime> _alertTimeQueue;

        private DateTime AlertTime => _alertTimeQueue.Peek();

        public TimeSpan RemainTime => GetRemainTime();

        private TimeSpan GetRemainTime()
        {
            var t = AlertTime - DateTime.Now;
            return t < TimeSpan.Zero ? TimeSpan.FromMilliseconds(100) : t;
        }

        public Guid Id { get; }

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

        /// <summary>
        ///     间隔时间
        /// </summary>
        private readonly TimeSpan _period;

        /// <summary>
        ///     重复
        /// </summary>
        private Times _remainTimes;

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alertTime">计时</param>
        /// <param name="remainTimes">剩下的执行次数</param>
        /// <param name="period">间隔时间</param>
        /// <param name="isAlarm"></param>
        /// <param name="message"></param>
        public TimingCommand(Guid id, DateTime alertTime, Times remainTimes, TimeSpan period, bool isAlarm,
            string message)
        {
            _remainTimes = remainTimes;
            _period = period;
            Id = id;
            IsAlarm = isAlarm;
            Message = message;
            _alertTimeQueue = new Queue<DateTime>();
            _alertTimeQueue.Enqueue(alertTime);
            for (var i = 0; i < remainTimes.Value - 1; i++)
            {
                _alertTimeQueue.Enqueue(_alertTimeQueue.Last().Add(period));
            }
        }

        public bool IsComplete => _remainTimes.IsOver;

        public void AfterRun()
        {
            _remainTimes.Reduce();
            _alertTimeQueue.Dequeue();
        }
    }
}