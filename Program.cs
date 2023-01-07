using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace az204_auth
{
    class Program
    {
        private const string _clientId = "f8585460-1520-4fc4-8339-e6e4f2629a46";
        private const string _tenantId = "a28947a6-4e31-461e-a387-8e0c8ed42b0b";

        public static async Task Main(string[] args)
        {
            var app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .WithRedirectUri("http://localhost")
                .Build();
            string[] scopes = { "user.read" };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            var graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                return Task.CompletedTask;
            }));

            // GET https://graph.microsoft.com/v1.0/me
            var user = await graphServiceClient.Me
                .Request()
                .GetAsync();
            // DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, graphScopes);
            // GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            Console.WriteLine($"Token:\t{result.ExpiresOn}");
            Console.WriteLine($"Display Name:\t{user.DisplayName}");
            Console.WriteLine($"UserPrincipalName:\t{user.UserPrincipalName}");
        }
    }
}