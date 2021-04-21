using DirectDo.Application;
using DirectDo.Domain.Models;
using DirectDo.Server.Notifiers;
using DirectDo.Server.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

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
                        .AddSingleton<IAlertService, AlertService>();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        services.AddSingleton<INotify, LinuxNotifier>();
                    }
                    else
                    {
                        services.AddSingleton<INotify, WindowsNotifier>();

                    }

                    services.BuildServiceProvider();
                });
        }
    }
}