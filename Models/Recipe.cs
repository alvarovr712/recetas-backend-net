using System;
using System.Collections.Generic;

using RecetasAPINet.Enums;

namespace RecetasAPINet.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }

        // Relación con User (creador)
        public Guid UserId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public RecipeType Type { get; set; }
        public int PrepTime { get; set; }
        public int Servings { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relación con ingredientes
        public List<RecipeIngredient> Ingredients { get; set; } = new();

        // Relación con pasos
        public List<Step> Steps { get; set; } = new();

        // Relación con favoritos (N:N)
        [System.Text.Json.Serialization.JsonIgnore]
        public List<UserFavorite> FavoritedBy { get; set; } = new();
    }
}
