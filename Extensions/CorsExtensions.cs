using Microsoft.Extensions.DependencyInjection;

namespace RecetasAPINet.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsForAngular(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
