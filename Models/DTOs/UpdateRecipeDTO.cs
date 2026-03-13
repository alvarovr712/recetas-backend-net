namespace RecetasAPINet.DTOs
{
    public class UpdateRecipeDTO
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int? PrepTime { get; set; }
        public int? Servings { get; set; }
        public string? Image { get; set; }

        public List<UpdateRecipeIngredientDTO>? Ingredients { get; set; }
        public List<UpdateRecipeStepDTO>? Steps { get; set; }
    }

    public class UpdateRecipeIngredientDTO
    {
        public string? IngredientId { get; set; }
        public string? Quantity { get; set; }
        public string? Unit { get; set; }
    }

    public class UpdateRecipeStepDTO
    {
        public int? StepOrder { get; set; }
        public string? Instruction { get; set; }
        public string? ImageStep { get; set; }
    }
}
