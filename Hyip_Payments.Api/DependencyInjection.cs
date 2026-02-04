using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Query.CoinQuery;
using Microsoft.Extensions.DependencyInjection;

namespace Hyip_Payments.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
        {
            // DbContext is already registered in Program.cs

            // Add MediatR - register ALL handlers from Command and Query assemblies
            services.AddMediatR(cfg =>
            {
                // Register all handlers from the Command assembly
                cfg.RegisterServicesFromAssembly(typeof(AddCoinCommand).Assembly);
                
                // Register all handlers from the Query assembly
                cfg.RegisterServicesFromAssembly(typeof(GetCoinsQuery).Assembly);
            });

            return services;
        }
    }
}
