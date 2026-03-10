using RecetasAPINet.Models;

public interface IRecipeIngredientRepository
{
    Task AddRangeAsync(List<RecipeIngredient> ingredients);
    
}