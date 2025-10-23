using Hyip_Payments.Api.Extensions;
using Hyip_Payments.Command.BrandCommand;
using Hyip_Payments.Command.CategoryCommand;
using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Command.ProductCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Query.BrandQuery;
using Hyip_Payments.Query.CategoryQuery;
using Hyip_Payments.Query.CoinQuery;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Query.InvoiceQuery;
using Hyip_Payments.Query.MoneyQuery;
using Hyip_Payments.Query.PaymentQuery;
using Hyip_Payments.Query.ProductQuery;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add PaymentsDbContext
            builder.Services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlServer(connectionString));
            
            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add Application Services
            builder.Services.AddApplicationServices(connectionString);

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Configure the exception page.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors();
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
