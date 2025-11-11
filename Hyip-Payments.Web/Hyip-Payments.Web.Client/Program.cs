using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Hyip_Payments.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthenticationStateDeserialization();

            // Register the AuthorizationMessageHandler
            builder.Services.AddScoped<AuthorizationMessageHandler>();

            // Register HttpClient with the authorization handler
            builder.Services.AddHttpClient("API", client =>
                client.BaseAddress = new Uri("https://localhost:7263"))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

            // Register default HttpClient that uses the named client
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

            await builder.Build().RunAsync();
        }
    }
}
