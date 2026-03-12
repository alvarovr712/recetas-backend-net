using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IRecipeService
    {
        Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request,Guid userId);
        Task<List<RecipeCardDto>> GetMisRecetasAsync(Guid userId, string? category = null);
        Task<RecipeDetailDto?> GetRecipeDetailAsync(Guid recipeId, Guid userId);
        Task<List<RecipeCardDto>> GetAllRecetasAsync(Guid userId, string? category = null);

        Task<bool> ToggleFavoriteAsync(Guid userId, Guid recipeId);
        Task<List<RecipeCardDto>> GetFavoritasAsync(Guid userId, string? category = null);

         Task<List<RecipeCardDto>> FiltroByTitleDescriptionOrIngredientAsync(Guid userId, string filtro);

         Task<List<RecipeCardDto>> FiltroMisRecetasAsync(Guid userId, string filtro);

         Task<List<RecipeCardDto>> FiltroFavoritasAsync(Guid userId, string filtro);
    }


       

    
}