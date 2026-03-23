using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.Repositories;
using RecetasAPINet.Security;

namespace RecetasAPINet.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Hash
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Cloudinary
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];
            
            if(!string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
            {
                var account = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
                var cloudinary = new CloudinaryDotNet.Cloudinary(account);
                services.AddSingleton(cloudinary);
            }

            // Servicios
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IRecipeService,RecipeService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IDashboardService,DashboardService>();

            //Repositorios
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
            services.AddScoped<IRecipeRepository,RecipeRepository>();
            services.AddScoped<IStepRepository,StepRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
            services.AddScoped<ILogRepository,LogRepository>();
            services.AddScoped<ISessionRepository,SessionRepository>();

            return services;
        }
    }
}
