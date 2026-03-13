using RecetasAPINet.Enums;

namespace RecetasAPINet.DTOs
{
    public class RecipeDetailDto
    {
        public Guid Id { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public RecipeType Type { get; set; }
        public int PrepTime { get; set; }
        public int Servings { get; set; }
        public string Description { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string UserImage { get; set; } = string.Empty;

        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
        public List<RecipeStepDto> Steps { get; set; } = new();
        public bool IsFavorite { get; set; }
    }

    public class RecipeIngredientDto
    {
        public Guid IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }

    public class RecipeStepDto
    {
        public int StepOrder { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
