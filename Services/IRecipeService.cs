using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IRecipeService
    {
        Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request,Guid userId);
        Task<List<RecipeCardDto>> GetMisRecetasAsync(Guid userId);
        Task<RecipeDetailDto?> GetRecipeDetailAsync(Guid recipeId, Guid userId);
        Task<List<RecipeCardDto>> GetAllRecetasAsync(Guid userId);

        Task<bool> ToggleFavoriteAsync(Guid userId, Guid recipeId);
        Task<List<RecipeCardDto>> GetFavoritasAsync(Guid userId);

       

    }
}