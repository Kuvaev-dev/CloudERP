using API.Factories;
using API.Helpers;
using API.Services;
using DatabaseAccess.DependencyInjection;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.DependencyInjection;
using System.Text;
using Utils.DependencyInjection;
using Domain.UtilsAccess;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Database Access
            builder.Services.AddDatabaseServices();
            // Utils
            builder.Services.AddUtilsServices();
            // Services
            builder.Services.AddServices();

            // Connetion String Provider
            builder.Services.AddScoped<IConnectionStringProvider, WebConfigConnectionStringProvider>();
            // Local Services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();
            // Local Adapters
            builder.Services.AddScoped<IFileAdapterFactory, FileAdapterFactory>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
