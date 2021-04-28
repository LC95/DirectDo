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
using DirectDo.Domain;

namespace DirectDo.Server
{
    public class Program
    {
        private static readonly Mutex Mutex;
        private static readonly bool IsCreatedNew;

        static Program()
        {
            Mutex = new Mutex(true, "Global\\DirectDo.Server", out IsCreatedNew);
        }

        public static void Main(string[] args)
        {
            if (IsCreatedNew)
            {
                Mutex.WaitOne();
                CreateHostBuilder(args).Build().Run();
                Mutex.ReleaseMutex();
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
                            Assembly.GetAssembly(typeof(AlertCommandRepository)),
                            Assembly.GetAssembly(typeof(INotify)))
                        .AddSingleton<IAlertCommandRepository, AlertCommandRepository>()
                        .AddSingleton<IClock, NockNock>();
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