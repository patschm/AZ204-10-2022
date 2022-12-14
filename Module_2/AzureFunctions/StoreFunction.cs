using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctions
{
    public static class StoreFunction
    {
        public class Person: TableEntity
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }
        
       const string conStr = "DefaultEndpointsProtocol=https;AccountName=myfunctionsstore;AccountKey=GDCGUf8Zqj5LRihrur8LCzBRbbgvJm5izy6gEZyTv/l7GOKKFdoCwEB4r6Ys9Ge4c7e2/0q0NoB1+AStETvN1w==;EndpointSuffix=core.windows.net";

        //Microsoft.Azure.WebJobs.Extensions.Storage <= v4.05
       [return: Table("people", Connection =conStr)]
       [FunctionName("StoreFunction")]
        public static Person Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "people/{first}/{last}/{age?}")] HttpRequest req,
            string first,
            string last,
            int? age,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new Person
            {
                PartitionKey = "viahttp",
                RowKey = Guid.NewGuid().ToString(),
                FirstName = first,
                LastName = last,
                Age = age ?? 42
            };

          
        }
    }
}
