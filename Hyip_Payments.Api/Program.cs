using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Query.CoinQuery;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Query.InvoiceQuery;
using Hyip_Payments.Query.MoneyQuery;
using Hyip_Payments.Query.PaymentQuery;
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
                cfg.RegisterServicesFromAssemblyContaining<GetAllInvoicesQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetInvoiceByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetMoneyQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetPaymentListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCointByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCoinsQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCountryByIdQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetCountryListQuery>();
                cfg.RegisterServicesFromAssemblyContaining<AddCountryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EditCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<DeleteCoinCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddInvoiceCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddMoneyCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddPaymentCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddUserCommand>();
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
