using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Hyip_Payments.Web.Client.Services;

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

            // Register Cart Service (Client-side WebAssembly - persists in browser memory during session)
            // Uses Singleton to maintain cart state across component navigations within the same session
            builder.Services.AddSingleton<ICartService, CartService>();

            // Register Auth Token Service (stores JWT token in localStorage)
            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

            // Option 1: Determine API base address based on environment (hardcoded)
            var apiBaseAddress = builder.HostEnvironment.IsDevelopment()
                ? "https://localhost:7263"  // Development
                : "https://mariobot-payments-api.runasp.net";  // Production

            // Option 2: Get from configuration (if you prefer config-based)
            // var apiBaseAddress = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7263";

            // Log environment and API address for debugging
            Console.WriteLine($"Environment: {builder.HostEnvironment.Environment}");
            Console.WriteLine($"API Base Address: {apiBaseAddress}");

            // HttpClient with conditional base address and automatic token injection
            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
                handler.InnerHandler = new HttpClientHandler();

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseAddress)
                };
            });

            await builder.Build().RunAsync();
        }
    }
}
