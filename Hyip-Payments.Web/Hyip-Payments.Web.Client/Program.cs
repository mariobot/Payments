using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Hyip_Payments.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Simple setup for server-side Identity authentication
            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthenticationStateDeserialization();

            string val = builder.HostEnvironment.BaseAddress;

            // HttpClient - use the Web app's base address (same origin)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5009")
            });

            await builder.Build().RunAsync();
        }
    }
}
