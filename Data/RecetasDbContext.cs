using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Models;
using RecetasAPINet.Enums;

namespace RecetasAPINet.Data
{
    public class RecetasDbContext : DbContext
    {
        public RecetasDbContext(DbContextOptions<RecetasDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }

        public DbSet<Log> Logs{get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Role>("role_enum", nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
            modelBuilder.HasPostgresEnum<RecipeType>("recipe_type_enum", nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());

            // Tabla user_favorites con clave compuesta
            modelBuilder.Entity<UserFavorite>()
                .ToTable("user_favorites")
                .HasKey(uf => new { uf.UserId, uf.RecipeId });

            // Relaciones UserFavorite
            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.Recipe)
                .WithMany(r => r.FavoritedBy)
                .HasForeignKey(uf => uf.RecipeId);

            // Tabla recipe_ingredients
            modelBuilder.Entity<RecipeIngredient>()
                .ToTable("recipe_ingredients");

            // Tabla sessions
            modelBuilder.Entity<Session>()
                .ToTable("sessions");

            base.OnModelCreating(modelBuilder);
        }
    }
}
