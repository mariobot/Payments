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

            // Add MediatR
            builder.Services.AddMediatR(cfg => {
                // Command assemblies
                cfg.RegisterServicesFromAssemblyContaining<AddCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddCountryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditCountryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteCountryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddInvoiceCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditInvoiceCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteInvoiceCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddMoneyCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditMoneyCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteMoneyCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddPaymentCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddUserCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddBrandCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditBrandCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteBrandCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddCategoryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditCategoryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteCategoryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddProductCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditProductCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteProductCommand>();

                // Query assemblies
                cfg.RegisterServicesFromAssemblyContaining<GetCoinsQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCointByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCountryListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCountryByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetAllInvoicesQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetInvoiceByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetBrandListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetBrandByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCategoriesListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCategoryByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetProductListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetProductByIdQuery>();
            });

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:7193", "http://localhost:5244", 
                        "https://localhost:5244", "http://localhost:65377", "http://localhost:7263"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

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
