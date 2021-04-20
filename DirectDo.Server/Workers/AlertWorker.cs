using System;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectDo.Server.Workers
{
    public class AlertWorker : BackgroundService
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertWorker> _logger;

        public AlertWorker(ILogger<AlertWorker> logger, IAlertService alertService)
        {
            _logger = logger;
            _alertService = alertService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AlertService running at: {time}", DateTimeOffset.Now);
            await Task.Run(() => _alertService.RunAlertAsync(stoppingToken), stoppingToken);
        }
    }
}