using Hyip_Payments.Api.Extensions;
using Hyip_Payments.Context;
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
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:5244", "https://localhost:7193", "https://localhost:44348/")
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

            //app.UseCors("AllowSpecificOrigins");
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
