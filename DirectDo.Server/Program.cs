using System;
using System.Reflection;
using System.Threading;
using DirectDo.Application;
using DirectDo.Domain.Models;
using DirectDo.Server.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DirectDo.Server.Notifiers;

namespace DirectDo.Server
{
    public class Program
    {
        private static readonly Mutex _mutex;
        private static readonly bool _isCreatedNew;

        static Program()
        {
            _mutex = new Mutex(true, "Global\\DirectDo.Server", out _isCreatedNew);
        }

        public static void Main(string[] args)
        {
            if (_isCreatedNew)
            {
                _mutex.WaitOne();
                CreateHostBuilder(args).Build().Run();
                _mutex.ReleaseMutex();
            }
            else
            {
                Console.WriteLine("已有一个DirectDo Server实例正在运行");
            }
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
                        .BuildServiceProvider();
#if WINDOWS10_0_19041_0
                    services.AddSingleton<INotify, WindowsNotifier>();
#else
                    services.AddSingleton<INotify, LinuxNotifier>();
#endif
                });
        }
    }
}