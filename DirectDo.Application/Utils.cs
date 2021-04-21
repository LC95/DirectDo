using System;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Events;
using DirectDo.Domain.Models;

namespace DirectDo.Application
{
    public static class Utils
    {

        private static Options OptionsParse(string[] args)
        {
            var option = new Options();
            foreach (var arg in args)
            {
                if (arg.StartsWith('@'))
                {
                    option.At = arg.Substring(1);
                }

                if (arg.StartsWith('>'))
                {
                    option.After = arg.Substring(1);
                }

                if (arg.StartsWith('!'))
                {
                    if (arg.Length == 1)
                    {
                        option.AlarmNumber = null;
                    }
                    else
                    {
                        option.AlarmNumber = int.Parse(arg.Substring(1));
                    }
                }

                if (arg.StartsWith('<'))
                {
                    option.MaintainTimes = int.Parse(arg.Substring(1));
                }

                if (arg.StartsWith('&'))
                {
                    option.Message = arg.Substring(1);
                }
            }
            return option;
        }

        private static IControlCommand BuildCommand(Options options)
        {
            DateTime? at = null;
            TimeSpan? after = null;
            var isAlarm = options.AlarmNumber != null;
            var maintainTimes = options.MaintainTimes;

            if (!string.IsNullOrEmpty(options.At)) at = DateTime.Parse(options.At);

            if (!string.IsNullOrEmpty(options.After)) after = Utils.ParsePeriod(options.After);

            if (at != null)
            {
                var cmd = new AtTimingCommand(at.Value, isAlarm, options.Message);
                return new TimingCreatedNotification(cmd);
            }

            if (after != null)
            {
                var next = DateTime.Now.Add(after.Value);
                var cmd = new PeriodTimingAlertCommand(next, new Times(maintainTimes), after.Value, isAlarm,
                    options.Message);
                return new TimingCreatedNotification(cmd);
            }

            if (string.IsNullOrEmpty(options.Search))
                return new SearchCommand(options.Search);
            if (string.IsNullOrWhiteSpace(options.Delete)) return new DeleteCommand(options.Delete);

            throw new ArgumentException("参数无法转换");
        }
        public static IControlCommand BuildCommand(string[] args){
            return BuildCommand(OptionsParse(args));
        }
        public static TimeSpan ParsePeriod(string s)
        {
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            var num = 0;
            foreach (var c in s)
                if (char.IsDigit(c))
                {
                    var n = int.Parse(c.ToString());
                    num = num * 10 + n;
                }
                else
                {
                    if (c == 'd')
                        days = num;
                    else if (c == 'h')
                        hours = num;
                    else if (c == 'm')
                        minutes = num;
                    else
                        seconds = num;

                    num = 0;
                }

            return new TimeSpan(days, hours, minutes, seconds);
        }
    }
}