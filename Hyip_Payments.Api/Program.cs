using System.Text;
using Hyip_Payments.Api.Extensions;
using Hyip_Payments.Command.ProductCommand;
using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Query.ProductQuery;
using Hyip_Payments.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
                options.UseSqlServer(connectionString, 
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));



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
            builder.Services.AddScoped<IRequestHandler<LoginUserCommand, UserModel?>, LoginUserCommandHandler>();

            // Uses Singleton to maintain cart state across component navigations within the same session
            //builder.Services.AddSingleton<ICartService, CartService>();

            // Add TokenService
            builder.Services.AddScoped<TokenService>();

            // Add DUAL Authentication: JWT Bearer (for API) + Cookie (for Blazor components)
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
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
                })
                .AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            builder.Services.AddAuthorization();

            // Add Application Services
            builder.Services.AddApplicationServices(connectionString);

            // Configure JSON serialization to handle reference loops
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Ignore circular references
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    // Optional: Make property names camelCase
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    // Optional: Write indented JSON for better readability
                    options.JsonSerializerOptions.WriteIndented = false;
                });

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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
