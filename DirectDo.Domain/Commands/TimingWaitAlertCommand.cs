using DirectDo.Domain.Models;
using System;

namespace DirectDo.Domain.Commands
{
    /// <summary>
    ///     普通的计时指令
    /// </summary>
    public class AtTimingCommand : TimingCommand
    {
        private bool _isComplete;

        public AtTimingCommand(DateTime alertTime, bool isAlarm, string message)
            : base(alertTime, isAlarm, message)
        {
            _isComplete = false;
        }

        public override bool IsComplete => _isComplete;

        public override void AfterRun()
        {
            _isComplete = true;
        }
    }
}