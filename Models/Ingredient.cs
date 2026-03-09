using System;
using System.Collections.Generic;

namespace RecetasAPINet.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
       

        // Relación con RecipeIngredient (1:N)
        public List<RecipeIngredient> Recipes { get; set; } = new();
    }
}
