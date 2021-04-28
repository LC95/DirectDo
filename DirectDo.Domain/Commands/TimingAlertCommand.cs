using DirectDo.Domain.Models;
using System;

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
    }

    /// <summary>
    ///     重复指令
    /// </summary>
    public class TimingAlertCommand : TimingCommand
    {
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
        /// <param name="Guid"></param>
        public TimingAlertCommand(Guid id, DateTime alertTime, Times remainTimes, TimeSpan period, bool isAlarm,
            string message)
            : base(id,alertTime, isAlarm, message)
        {
            _remainTimes = remainTimes;
            _period = period;
        }

        public override bool IsComplete => _remainTimes.IsOver;

        public override void AfterRun()
        {
            _remainTimes.Reduce();
            AlertTime = AlertTime.Add(_period);
        }
    }
}