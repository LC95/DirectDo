using System.Reflection;
using DirectDo.Application;
using DirectDo.Application.Utils;
using DirectDo.Domain.Models;
using DirectDo.Service.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DirectDo.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddHostedService<AlertWorker>()
                        .AddHostedService<CommandReceiverWorker>()
                        .AddMediatR(
                            Assembly.GetAssembly(typeof(AlertService)),
                            Assembly.GetAssembly(typeof(INotify)))
                        .AddSingleton<IAlertService, AlertService>()
                        .AddSingleton<INotify, LinuxNotifier>()
                        .BuildServiceProvider();
                });
        }
    }
}