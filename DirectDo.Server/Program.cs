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
using NetMQ;
using NetMQ.Sockets;

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
                        .AddHostedService<CommandReceiverWorker>()
                        .AddMediatR(
                            Assembly.GetAssembly(typeof(AlertCommandRepository)),
                            Assembly.GetAssembly(typeof(INotifier)))
                        .AddSingleton<IAlertCommandRepository, AlertCommandRepository>()
                        .AddSingleton<IServerMessenger, ServerMessenger>()
                        .AddSingleton<NetMQRuntime>()
                        .AddSingleton<RouterSocket>(new RouterSocket("@tcp://127.0.0.1:5556"));
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        services.AddSingleton<INotifier, LinuxNotifier>();
                    }
                    else
                    {
                        services.AddSingleton<INotifier, WindowsNotifier>();
                    }

                    services.BuildServiceProvider();
                });
        }
    }
}