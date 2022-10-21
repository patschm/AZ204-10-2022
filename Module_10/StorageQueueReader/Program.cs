using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace StorageQueueReader
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=hubstoor;AccountKey=rs2TGfH9jPhnZLt1rN1aAAQ9gPmCEnpA2j2+2XQtl937pfi9S8CkoDC6cEx8AIWNgznnbjeGXT4O+AStkM6Y6g==;EndpointSuffix=core.windows.net";
        static string QueueName = "simplequeue";
        static async Task Main(string[] args)
        {
            await ReadFromQueueAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task ReadFromQueueAsync()
        {
            var cnt = 0;
            var client = new QueueClient(ConnectionString, QueueName);
            do
            {
                // 10 seconds "lease" time
                var response = await client.ReceiveMessageAsync(TimeSpan.FromSeconds(10));
                if (response.Value == null)
                {
                    await Task.Delay(100);
                    continue;
                }
                var msg = response.Value;
                Console.WriteLine($"[{++cnt}] {msg.Body}");
              
                // We need more time to process
                //await client.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.Body, TimeSpan.FromSeconds(30));
                // Don't forget to remove
                await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            }
            while (true);
        }
    }
}
