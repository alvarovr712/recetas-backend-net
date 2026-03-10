using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IRecipeService
    {
        Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request,Guid userId);
        Task<List<RecipeCardDto>> GetMisRecetasAsync(Guid userId);
        Task<RecipeDetailDto?> GetRecipeDetailAsync(Guid recipeId);
    }
}