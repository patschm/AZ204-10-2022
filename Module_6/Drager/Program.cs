using System.Net.Http.Headers;
using Microsoft.Identity.Client;

//ConfidentialClientApplicationBuilder.Create("").WithClientSecret().Build();
var app = PublicClientApplicationBuilder
    .Create("b79d968c-eb4b-4b5c-a502-b0d060ec5b2a")
    .WithAuthority(AzureCloudInstance.AzurePublic, "common")
    .WithRedirectUri("http://localhost/");

    var token = await app.Build()
   // .AcquireTokenByUsernamePassword
    .AcquireTokenInteractive(new string[]{ "api://b79d968c-eb4b-4b5c-a502-b0d060ec5b2a/MijnApi" })
    .ExecuteAsync();
    System.Console.WriteLine(token.AccessToken);

    var client = new HttpClient();
    client.BaseAddress = new Uri("https://localhost:7241/");

    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token.AccessToken);

    string data = await client.GetStringAsync("weatherforecast");
    System.Console.WriteLine(data);

    Console.ReadLine();