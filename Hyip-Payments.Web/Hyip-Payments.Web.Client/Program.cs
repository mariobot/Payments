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

            // Registers HttpClient with the base address of the API
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5009") });

            await builder.Build().RunAsync();
        }
    }
}
