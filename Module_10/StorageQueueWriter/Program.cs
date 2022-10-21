using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace StorageQueueWriter
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=hubstoor;AccountKey=rs2TGfH9jPhnZLt1rN1aAAQ9gPmCEnpA2j2+2XQtl937pfi9S8CkoDC6cEx8AIWNgznnbjeGXT4O+AStkM6Y6g==;EndpointSuffix=core.windows.net";
        static string QueueName = "simplequeue";
        static async Task Main(string[] args)
        {
            await WriteToQueueAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task WriteToQueueAsync()
        {
            var client = new QueueClient(ConnectionString, QueueName);
            for (int i = 0; i < 100; i++)
            {
                await client.SendMessageAsync($"Hello Number {i}", timeToLive:TimeSpan.FromSeconds(20));
            }
            
        }

    }
}
