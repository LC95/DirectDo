using CommandLine;

namespace DirectDo.Application
{
    public class Options
    {
        [Option('!', "alarm", Required = false, HelpText = "EnableAlarm")]
        public int? AlarmNumber { get; set; }

        [Option('@', "at", Required = false, HelpText = "Alert at this time",
            SetName = "Timing")]
        public string At { get; set; }

        [Option('>', "after", Required = true, HelpText = "Alert after a period",
            SetName = "PeriodTiming")]
        public string After { get; set; }

        [Option('<', "maintain", Default = 1, HelpText = "Do a lot of times",
            SetName = "PeriodTiming")]
        public int MaintainTimes { get; set; }

        [Option('s', "search", HelpText = "Input Search Text", SetName = "Search")]
        public string Search { get; set; }

        [Option('d', "delete", HelpText = "delete", SetName = "Delete")]
        public string Delete { get; set; }

        [Option('m', "message", Required = true, HelpText = "Leave a message")]
        public string Message { get; set; }
    }
}