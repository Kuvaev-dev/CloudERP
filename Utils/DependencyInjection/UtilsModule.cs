using Microsoft.Extensions.DependencyInjection;
using Domain.UtilsAccess;
using Utils.Helpers;

namespace Utils.DependencyInjection
{
    public static class UtilsModule
    {
        public static IServiceCollection AddUtilsServices(this IServiceCollection services)
        {
            // Password Helper
            services.AddScoped<IPasswordHelper, PasswordHelper>();

            return services;
        }
    }
}
