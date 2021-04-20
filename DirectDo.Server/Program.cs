using System;
using System.Reflection;
using System.Threading;
using DirectDo.Application;
using DirectDo.Application.Utils;
using DirectDo.Domain.Models;
using DirectDo.Server.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                        .AddSingleton<INotify, LinuxNotifier>()
                        .BuildServiceProvider();
                });
        }
    }
}