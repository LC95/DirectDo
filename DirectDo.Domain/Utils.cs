using System;

namespace DirectDo.Domain
{
    public static class Utils
    {
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