using System.Net.Http;
using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Command.InvoiceItemCommand;
using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CoinQuery;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Query.InvoiceQuery;
using Hyip_Payments.Query.MoneyQuery;
using Hyip_Payments.Query.PaymentQuery;
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
            builder.Services.AddScoped<IRequestHandler<AddUserCommand, UserModel>, AddUserCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditUserCommand, UserModel?>, EditUserCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();

            // Payment Commands
            builder.Services.AddScoped<IRequestHandler<AddPaymentCommand, PaymentModel>, AddPaymentCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditPaymentCommand, PaymentModel?>, EditPaymentCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeletePaymentCommand, bool>, DeletePaymentCommandHandler>();

            // Money Commands
            builder.Services.AddScoped<IRequestHandler<AddMoneyCommand, MoneyModel>, AddMoneyCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditMoneyCommand, MoneyModel?>, EditMoneyCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteMoneyCommand, bool>, DeleteMoneyCommandHandler>();

            // Invoice Commands
            builder.Services.AddScoped<IRequestHandler<AddInvoiceCommand, InvoiceModel>, AddInvoiceCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditInvoiceCommand, InvoiceModel?>, EditInvoiceCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteInvoiceCommand, bool>, DeleteInvoiceCommandHandler>();

            // InvoiceItem Commands
            builder.Services.AddScoped<IRequestHandler<AddInvoiceItemCommand, InvoiceItemModel>, AddInvoiceItemCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditInvoiceItemCommand, InvoiceItemModel?>, EditInvoiceItemCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteInvoiceItemCommand, bool>, DeleteInvoiceItemCommandHandler>();

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

            return services;
        }
    }
}
