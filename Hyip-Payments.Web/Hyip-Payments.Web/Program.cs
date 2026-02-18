using CryptoExchange.Net.Authentication;
using Hyip_Payments.Context;
using Hyip_Payments.Services;
using Hyip_Payments.Web.Client.Services;
using Hyip_Payments.Web.Components;
using Hyip_Payments.Web.Components.Account;
using Hyip_Payments.Web.Data;
using Hyip_Payments.Web.Extensions;
using Hyip_Payments.Web.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            // Configure API endpoint based on environment
            var apiEndpoint = builder.Environment.IsDevelopment()
                ? "https://localhost:7263"  // Development
                : "https://mariobot-payments-api.runasp.net";  // Production

            // Add HttpClient for WebAssembly components with conditional base address
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri(apiEndpoint)
            });

            // Identity services 
            // TODO pending while fix the migrations of the Identity
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();

            // TODO coment while fix the migrations of the Identity
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add PaymentsDbContext
            builder.Services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlServer(connectionString, 
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

            // Add ApplicationDbContext
            // TODO pending while fix the migrations of the Identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, 
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

            var apiKeyBinance = builder.Configuration.GetSection("Binance").GetRequiredSection("ApiKey").Value;
            var apiSecretBinance = builder.Configuration.GetSection("Binance").GetRequiredSection("ApiSecret").Value;

            builder.Services.AddBinance(restClientOptions =>
            {
                restClientOptions.ApiCredentials = new ApiCredentials(apiKeyBinance, apiSecretBinance);
                restClientOptions.Rest.OutputOriginalData = true;
            });


            // If the method is part of a library, ensure the library is installed via NuGet:
            // Example: Install-Package YourLibraryName

            // If the method is not available, you may need to implement it or provide an alternative.


            // Add CORS policy with environment-based configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.WithOrigins("https://localhost:7263", "http://localhost:5000")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.WithOrigins("https://mariobot-payments-api.runasp.net")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                });
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // TODO pendig while fix the migrations of the Identity
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>(); // Add custom claims factory

            // TODO pending identity migrations
            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            // Register TokenService for JWT token generation (shared with API)
            builder.Services.AddScoped<TokenService>();

            // Register Cart Service (Server-side - persists during session)
            builder.Services.AddSingleton<ICartService, CartService>();

            // Register Auth Token Service (Server-side - for JWT token management)
            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

            // Register Authorization Message Handler (Server-side - adds Bearer token to HTTP requests)
            builder.Services.AddScoped<AuthorizationMessageHandler>();

            // Use the already configured API endpoint
            builder.Services.AddWebApplicationServices(apiEndpoint);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseDeveloperExceptionPage();
                //app.UseMigrationsEndPoint();
            }
            else
            {
                // TODO remove in production
                app.UseDeveloperExceptionPage();

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            // Cors alwais is difined later up httpredirection
            //app.UseCors("AllowSpecificOrigins");

            app.MapStaticAssets();
            
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
