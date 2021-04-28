using DirectDo.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain;

namespace DirectDo.Server.Workers
{
    public class AlertWorker : BackgroundService
    {
        private readonly IClock _clock;
        private readonly ILogger<AlertWorker> _logger;

        public AlertWorker(ILogger<AlertWorker> logger, IClock clock)
        {
            _logger = logger;
            _clock = clock;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AlertService running at: {time}", DateTimeOffset.Now);
            await _clock.BeginWaitAsync();
        }
    }
}