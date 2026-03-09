using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> BuscarTodos();
        Task<Ingredient> CrearIngrediente(string nombre);
    }
}