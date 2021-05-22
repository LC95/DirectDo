using DirectDo.Domain.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MediatR;
using Timer = System.Timers.Timer;
using Microsoft.VisualStudio.Threading;

namespace DirectDo.Application
{
    public class TimerZ : IDisposable
    {
        private readonly IMediator _mediator;
        public TimingCommand Command { get; }
        private readonly Timer _timer;

        public TimerZ(TimingCommand command, IMediator mediator)
        {
            _mediator = mediator;
            Command = command;
            _timer = new Timer {Interval = command.RemainTime.Milliseconds, AutoReset = false};
            _timer.Elapsed +=   async ( _, _ ) => await RunAsync(Command);
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task RunAsync(TimingCommand c)
        {
            try
            {
                await _mediator.Publish(c);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class AlertCommandRepository : IAlertCommandRepository
    {
        private readonly IMediator _mediator;

        //等待的通知队列
        private readonly Dictionary<Guid, TimerZ> _commands = new();

        public AlertCommandRepository(IMediator mediator)
        {
            _mediator = mediator;
        }
        

        public void AddCommand(TimingCommand timingCommand)
        {
            _commands.Add(timingCommand.Id, new TimerZ(timingCommand, _mediator));
        }

        public void RemoveCommand(Guid id)
        {
            if (_commands.TryGetValue(id, out var t))
            {
                t.Dispose();
                _commands.Remove(id);
            }
        }

        public void UpdateCommand(TimingCommand command)
        {
            return;
        }

        public TimingCommand Find(Guid id)
        {
            return _commands.TryGetValue(id, out var value) ? value.Command : null;
        }

        public IEnumerable<TimingCommand> All()
        {
            return _commands.Values.Select(t => t.Command);
        }
    }
}