using RecetasAPINet.Enums;

public class RecipeCardDto
{
    public Guid Id { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecipeType Type { get; set; }
}
