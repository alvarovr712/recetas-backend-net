using RecetasAPINet.Models;
using RecetasAPINet.Repositories;

namespace RecetasAPINet.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repo;

        public IngredientService(IIngredientRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Ingredient>> BuscarTodos()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Ingredient> CrearIngrediente(string nombre)
        {
            var ingrediente = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = nombre
            };
            await _repo.AddAsync(ingrediente);
            return ingrediente;
        }
    }
}