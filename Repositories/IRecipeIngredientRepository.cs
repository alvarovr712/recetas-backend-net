using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IRecipeIngredientRepository
    {
        Task AddRangeAsync(List<RecipeIngredient> ingredients);
        Task<List<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId);

        Task<List<RecipeIngredient>> GetByIngredientIdsAsync(List<Guid> ingredientIds);


    }
}