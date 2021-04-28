using System;
using System.Threading.Tasks;

namespace DirectDo.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var p = new ClientMessenger();

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