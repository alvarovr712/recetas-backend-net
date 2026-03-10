using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IRecipeIngredientRepository
    {
        Task AddRangeAsync(List<RecipeIngredient> ingredients);

    }
}