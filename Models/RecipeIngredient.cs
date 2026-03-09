using System;

namespace RecetasAPINet.Models
{
    public class RecipeIngredient
    {
        public Guid Id { get; set; }

        // Relación con Ingredient
        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        // Relación con Recipe
        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public double Quantity { get; set; }
    }
}
