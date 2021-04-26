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
                    option.At = arg[1..];
                }

                if (arg.StartsWith('>'))
                {
                    option.After = arg[1..];
                }

                if (arg.StartsWith('!'))
                {
                    option.AlarmNumber = arg.Length == 1 ? 1 : int.Parse(arg[1..]);
                }

                if (arg.StartsWith('#'))
                {
                    option.ReqId = Guid.Parse(arg[1..]);
                }

                if (arg.StartsWith('<'))
                {
                    option.MaintainTimes = int.Parse(arg[1..]);
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
            var id = options.ReqId;
            var alarm = options.AlarmNumber;
            var maintainTimes = options.MaintainTimes;


            if (!string.IsNullOrEmpty(options.At)) at = DateTime.Parse(options.At);

            if (!string.IsNullOrEmpty(options.After)) after = ParsePeriod(options.After);

            if (after != null || at != null)
            {
                //没有传计时
                if (after != null)
                {
                    at ??= DateTime.Now.Add(after.Value);
                }

                after ??= TimeSpan.Zero;
                var cmd = new TimingAlertCommand(id, at.Value, new Times(maintainTimes), after.Value, alarm,
                    options.Message);
                return new TimingCreatedNotification(cmd);
            }


            if (string.IsNullOrEmpty(options.Search))
                return new SearchCommand(options.Search);
            if (string.IsNullOrWhiteSpace(options.Delete)) return new DeleteCommand(options.Delete);

            throw new ArgumentException("参数无法转换");
        }

        public static IControlCommand BuildCommand(string[] args)
        {
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

            if (num != 0)
            {
                seconds = num;
            }

            return new TimeSpan(days, hours, minutes, seconds);
        }
    }
}