
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
#endregion

#region AppConfiguration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Azure;
#endregion


namespace KeyVault
{
    class Program
    {
        static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        static string clientId = "e568c1dc-ff6d-4086-a0a5-366f38491a97";
        static string clientSecret = "5an8Q~zn9BtBYjW7U55tiscA5DtPS-JwrmX-wcYn";
        static string kvUri = "https://ps-thekeys.vault.azure.net/";
        
        static async Task Main(string[] args)
        {
           //await ReadKeyVault();
           await ReadAppConfigurationAsync();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        private static async Task ReadKeyVault()
        {
            ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);
                
            var result = await kvClient.GetSecretAsync("MyKey");
            Console.WriteLine($"Hello {result.Value?.Value}");
        }

        private static async Task ReadAppConfigurationAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

                      

            //ReadLocal();
            await ReadRemoteAsync();

            void ReadLocal()
            {
                Console.WriteLine(configuration["MySetings:hello"]);
                Console.WriteLine(configuration["ConnectionString"]);
            }

            async Task ReadRemoteAsync()
            {
                ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);

                builder.AddAzureKeyVault(new Uri(kvUri), cred);
                builder.AddAzureAppConfiguration(opts => {
                    opts.Connect(configuration["ConnectionString"])
                       .Select(KeyFilter.Any, "Production")
                       // .Select(KeyFilter.Any, "Prog") // When using labels in your configuration, import the appropriate keys for that label
                       .UseFeatureFlags();
                        
                    });
                //builder.AddAzureAppConfiguration(opts => {
                //    opts.ConfigureKeyVault(kvopts =>
                //    {
                //        kvopts.SetCredential(new ClientSecretCredential(tenentId, clientId, clientSecret));
                //    })
                //    .UseFeatureFlags();
                //    opts.Connect(configuration["ConnectionString"]);    
                   
               // });
                IConfiguration conf = builder.Build();

                Console.WriteLine($"{conf["KeyVault:MySetting:Hello"]}");
               // Console.WriteLine($"Hello {conf["Test"]}");

                IServiceCollection services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

                using (var svcProvider = services.BuildServiceProvider())
                {
                    var featureManager = svcProvider.GetRequiredService<IFeatureManager>();
                    if (await featureManager.IsEnabledAsync("feat1"))
                    {
                        Console.WriteLine("We have a new feature");
                    }
                }

            }
        }

    }
}
