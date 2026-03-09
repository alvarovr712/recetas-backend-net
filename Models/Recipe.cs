using System;
using System.Collections.Generic;

namespace RecetasAPINet.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }

        // Relación con User (creador)
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // o enum si lo usas
        public int PrepTime { get; set; }
        public int Servings { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        // Relación con ingredientes
        public List<RecipeIngredient> Ingredients { get; set; } = new();

        // Relación con pasos
        public List<Step> Steps { get; set; } = new();

        // Relación con favoritos (N:N)
        public List<UserFavorite> FavoritedBy { get; set; } = new();
    }
}
