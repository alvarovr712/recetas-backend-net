using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.Security;

namespace RecetasAPINet.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Hash
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Servicios de la aplicación
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
