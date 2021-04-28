using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using DirectDo.Domain.Commands;
using DirectDo.Domain.Events;
using DirectDo.Domain.Models;

namespace DirectDo.Application
{
    public static class Utils
    {
        public static IControlCommand BuildCommand(AddOptions addOptions)
        {
            DateTime? at = null;
            TimeSpan? after = null;
            var id = addOptions.ReqId;
            var alarm = addOptions.Sound;
            var maintainTimes = addOptions.Times;


            if (!string.IsNullOrEmpty(addOptions.At)) at = DateTime.Parse(addOptions.At);

            if (!string.IsNullOrEmpty(addOptions.After)) after = ParsePeriod(addOptions.After);

            if (after != null || at != null)
            {
                //没有传计时
                if (after != null)
                {
                    at ??= DateTime.Now.Add(after.Value);
                }

                after ??= TimeSpan.Zero;
                var cmd = new TimingAlertCommand(id, at.Value, new Times(maintainTimes), after.Value, alarm,
                    addOptions.Message);
                return new TimingCreatedNotification(cmd);
            }

            throw new ArgumentException("参数无法转换");
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

        public static RootCommand BuildRootCommand(Func<AddOptions, Task> func)
        {
            var rootCommand = new RootCommand();

            var addCommand = new Command("add", "添加一个闹钟") {TreatUnmatchedTokensAsErrors = true};
            addCommand.AddAlias("+");
            var messageArg = new Argument("message") {Description = "添加提醒信息"};

            var atArg = new Argument("at") {Description = "提醒时间, 可以不填，默认为现在", Arity = ArgumentArity.ZeroOrOne};
            
            addCommand.AddArgument(messageArg);
            addCommand.AddArgument(atArg);
            addCommand.Handler = CommandHandler.Create(func);
            addCommand.AddOption(new Option(new[] {"--sound", "-s"}, "启用提醒声音"));
            addCommand.AddOption(new Option(new[] {"--after", "-p"}, "等待一段时间", typeof(string)));
            addCommand.AddOption(new Option(new[] {"--times", "-t"}, "持续次数", typeof(int)));

            var searchCommand = new Command("search", "搜索全部闹钟");

            var deleteCommand = new Command("delete", "删除一个闹钟, 未完成");
            deleteCommand.AddOption(new Option(new[] {"--reqid", "-i"}));

            rootCommand.Add(addCommand);
            rootCommand.Add(deleteCommand);
            rootCommand.Add(searchCommand);
            return rootCommand;
        }
    }
}