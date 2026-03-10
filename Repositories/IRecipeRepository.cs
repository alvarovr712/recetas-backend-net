using RecetasAPINet.Models;

public interface IRecipeRepository
{
    Task AddAsync(Recipe recipe);
}