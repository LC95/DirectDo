using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using DirectDo.Application;

namespace DirectDo.Client
{
    internal class Program
    {
      

     

        private static async Task Main(string[] args)
        {
            var p = new MessageSender();

            try
            {
                await p.SendMessageAsync(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

  
    }
}