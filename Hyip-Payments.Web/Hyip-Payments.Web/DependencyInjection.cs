using System.Net.Http;
using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Command.InvoiceItemCommand;
using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Command.WalletCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CoinQuery;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Query.InvoiceQuery;
using Hyip_Payments.Query.MoneyQuery;
using Hyip_Payments.Query.PaymentQuery;
using Hyip_Payments.Query.WalletQuery;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hyip_Payments.Web.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, string apiBaseAddress)
        {
            // Add MediatR

            // User Commands
            services.AddScoped<IRequestHandler<AddUserCommand, UserModel>, AddUserCommandHandler>();
            services.AddScoped<IRequestHandler<EditUserCommand, UserModel?>, EditUserCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();

            // Payment Commands
            services.AddScoped<IRequestHandler<AddPaymentCommand, PaymentModel>, AddPaymentCommandHandler>();
            services.AddScoped<IRequestHandler<EditPaymentCommand, PaymentModel?>, EditPaymentCommandHandler>();
            services.AddScoped<IRequestHandler<DeletePaymentCommand, bool>, DeletePaymentCommandHandler>();

            // Money Commands
            services.AddScoped<IRequestHandler<AddMoneyCommand, MoneyModel>, AddMoneyCommandHandler>();
            services.AddScoped<IRequestHandler<EditMoneyCommand, MoneyModel?>, EditMoneyCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteMoneyCommand, bool>, DeleteMoneyCommandHandler>();

            // Invoice Commands
            services.AddScoped<IRequestHandler<AddInvoiceCommand, InvoiceModel>, AddInvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<EditInvoiceCommand, InvoiceModel?>, EditInvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteInvoiceCommand, bool>, DeleteInvoiceCommandHandler>();

            // InvoiceItem Commands
            services.AddScoped<IRequestHandler<AddInvoiceItemCommand, InvoiceItemModel>, AddInvoiceItemCommandHandler>();
            services.AddScoped<IRequestHandler<EditInvoiceItemCommand, InvoiceItemModel?>, EditInvoiceItemCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteInvoiceItemCommand, bool>, DeleteInvoiceItemCommandHandler>();

            // Wallet Commands
            services.AddScoped<IRequestHandler<AddWalletCommand, WalletModel>, AddWalletCommandHandler>();
            services.AddScoped<IRequestHandler<EditWalletCommand, WalletModel?>, EditWalletCommandHandler>();
            services.AddScoped<IRequestHandler<DisableWalletCommand, bool>, DisableWalletCommandHandler>();

            // PaymentTransaction Commands
            services.AddScoped<IRequestHandler<AddPaymentTransactionCommand, PaymentTransactionModel>, AddPaymentTransactionCommandHandler>();

            // Register all queries. 
            services.AddMediatR(cfg => {
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
                // Wallet Queries
                cfg.RegisterServicesFromAssemblyContaining<GetWalletQuery>();
                // Add more as needed for other queries/commands
            });

            return services;
        }
    }
}
