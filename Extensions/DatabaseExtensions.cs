using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RecetasAPINet.Data;
using RecetasAPINet.Enums;

namespace RecetasAPINet.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Crear DataSource con mapeo de enums
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));
            dataSourceBuilder.MapEnum<Role>("role_enum", new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
            dataSourceBuilder.MapEnum<RecipeType>("recipe_type_enum", new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
            var dataSource = dataSourceBuilder.Build();

            // Registrar DbContext
            services.AddDbContext<RecetasDbContext>(options =>
                options.UseNpgsql(dataSource)
                       .UseSnakeCaseNamingConvention());

            return services;
        }
    }
}
