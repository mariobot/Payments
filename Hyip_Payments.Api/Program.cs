using Hyip_Payments.Api.Extensions;
using Hyip_Payments.Command.ProductCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Query.ProductQuery;
using MediatR;
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

            var defaultEndpoint = builder.Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("default endpoint.");


            // Add PaymentsDbContext
            builder.Services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlServer(connectionString));



            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddScoped<IRequestHandler<AddProductCommand, ProductModel>, AddProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditProductCommand, ProductModel?>, EditProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteProductCommand, bool>, DeleteProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<GetProductListQuery, List<ProductModel>>, GetProductListQueryHandler>();
            builder.Services.AddScoped<IRequestHandler<GetProductByIdQuery, ProductModel?>, GetProductByIdQueryHandler>();
            builder.Services.AddScoped<IRequestHandler<RegisterUserCommand, UserModel>, RegisterUserCommandHandler>();


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

            app.UseCors("AllowAllOrigins");
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
