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
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Hash
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Servicios
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IRecipeService,RecipeService>();
            services.AddScoped<IImageService, ImageService>();

            //Repositorios
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
            services.AddScoped<IRecipeRepository,RecipeRepository>();
            services.AddScoped<IStepRepository,StepRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();

            return services;
        }
    }
}
