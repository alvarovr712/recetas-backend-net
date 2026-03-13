namespace RecetasAPINet.DTOs
{
    public class CreateRecipeRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public int PrepTime { get; set; }

        public int Servings { get; set; } = 4;

        public string Image { get; set; } = string.Empty;

        public List<CreateRecipeIngredientDto> Ingredients { get; set; } = new();
        public List<CreateRecipeStepDto> Steps { get; set; } = new();
    }
}

public class CreateRecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateRecipeStepDto
{
    public int StepOrder { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public string ImageStep { get; set; } = string.Empty;
}
