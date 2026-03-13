using System;

namespace RecetasAPINet.Models
{
    public class Step
    {
        public Guid Id { get; set; }

        // Relación con Recipe
        public Guid RecipeId { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public Recipe? Recipe { get; set; }

        public int StepOrder { get; set; }
        public string Instruction { get; set; } = string.Empty;

        public string? Image {get; set;} = string.Empty;
    }
}
