using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IRecipeService
    {
        Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request,Guid userId);
    }
}