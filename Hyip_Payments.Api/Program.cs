using Hyip_Payments.Api.Extensions;
using Hyip_Payments.Command.ProductCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Query.ProductQuery;
using Hyip_Payments.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                    policy.WithOrigins(
                            "https://localhost:7263",  // Web app HTTPS
                            "http://localhost:5009",   // Web app HTTP
                            "https://localhost:5009")  // Web app HTTPS (added)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });


            builder.Services.AddScoped<IRequestHandler<AddProductCommand, ProductModel>, AddProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<EditProductCommand, ProductModel?>, EditProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteProductCommand, bool>, DeleteProductCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<GetProductListQuery, List<ProductModel>>, GetProductListQueryHandler>();
            builder.Services.AddScoped<IRequestHandler<GetProductByIdQuery, ProductModel?>, GetProductByIdQueryHandler>();
            builder.Services.AddScoped<IRequestHandler<RegisterUserCommand, UserModel>, RegisterUserCommandHandler>();
            builder.Services.AddScoped<IRequestHandler<LoginUserCommand, UserModel?>, LoginUserCommandHandler>();

            // Add TokenService
            builder.Services.AddScoped<TokenService>();

            // TODO: Temporarily disabled JWT Authentication for testing
            // Re-enable once client authentication is properly configured
            /*
            // Add JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            builder.Services.AddAuthorization();
            */

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

            // TODO: Temporarily disabled authentication middleware
            // Re-enable once JWT authentication is properly configured
            // app.UseAuthentication();
            // app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
