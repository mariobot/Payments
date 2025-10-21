using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CoinQuery;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Query.InvoiceQuery;
using Hyip_Payments.Query.MoneyQuery;
using Hyip_Payments.Query.PaymentQuery;
using Hyip_Payments.Web.Components;
using Hyip_Payments.Web.Components.Account;
using Hyip_Payments.Web.Data;
using MediatR;
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

            // Add HttpClient for WebAssembly components
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri("https://localhost:7263")
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

            // Add ApplicationDbContext
            // TODO pending while fix the migrations of the Identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add PaymentsDbContext
            builder.Services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add MediatR

            // Only needed if you want to resolve the handler directly, not for MediatR
            builder.Services.AddScoped<IRequestHandler<AddCountryCommand, CountryModel>, AddCountryCommandHandler>();
            
            builder.Services.AddScoped<IRequestHandler<AddCoinCommand, CoinModel>, AddCoinCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditCoinCommand, CoinModel>, EditCoinCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteCoinCommand, bool>, DeleteCoinCommandHandler>();

            builder.Services.AddScoped<IRequestHandler<AddInvoiceCommand, InvoiceModel>, AddInvoiceCommandHandler>();
            
            builder.Services.AddScoped<IRequestHandler<AddMoneyCommand, MoneyModel>, AddMoneyCommandHandler>();

            builder.Services.AddScoped<IRequestHandler<AddPaymentCommand, PaymentModel>, AddPaymentCommandHandler>();

            builder.Services.AddScoped<IRequestHandler<AddUserCommand, UserModel>, AddUserCommandHandler>();

            //builder.Services.AddScoped<IAddCoinCommand, AddCoinCommand>();
            //builder.Services.AddScoped<IEditCoinCommand, EditCoinCommand>();
            //builder.Services.AddScoped<IAddCountryCommand, AddCountryCommand>();
            //builder.Services.AddScoped<IAddInvoiceCommand, AddInvoiceCommand>();
            //builder.Services.AddScoped<IAddMoneyCommand, AddMoneyCommand>();
            //// Other command registrations
            //builder.Services.AddScoped<IAddCountryCommand, AddCountryCommand>();
            //builder.Services.AddScoped<IAddPaymentCommandHandler, AddPaymentCommandHandler>();

            // Register all queries. 
            builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblyContaining<GetAllInvoicesQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetInvoiceByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCointByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCoinsQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCountryByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<AddCountryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditCoinCommand>();
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
                .AddDefaultTokenProviders();

            // TODO pending identity migrations
            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                //app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

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
