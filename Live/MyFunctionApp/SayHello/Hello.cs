using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SayHello
{
    public static class Hello
    {
        [FunctionName("Hello")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            var v1 = context.CallActivityAsync<string>(nameof(SayHello), "Tokyo");

           // await context.WaitForExternalEvent("manager");
            var v2 = context.CallActivityAsync<string>(nameof(SayHello), "Seattle");
            var v3 = context.CallActivityAsync<string>(nameof(SayHello), "London");

            await Task.WhenAll(v1, v2, v3);

            outputs.AddRange(new string[] { v1.Result, v2.Result, v3.Result });
            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public static async Task<string> SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            await Task.Delay(5000);
            return $"Hello {name}!";
        }

        [FunctionName("Hello_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Hello", "mijn");

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}