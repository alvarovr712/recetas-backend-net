using Microsoft.Extensions.Configuration.UserSecrets;
using RecetasAPINet.Data;
using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecetasDbContext _context;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRecipeIngredientRepository _recipeIngredientRepo;
        private readonly IStepRepository _stepRepo;

        public RecipeService(RecetasDbContext context, IRecipeRepository recipeRepo, IRecipeIngredientRepository recipeIngredientRepo,
        IStepRepository stepRepo)
        {
            _context = context;
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _stepRepo = stepRepo;
        
        }

        public async Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request, Guid userId)
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                PrepTime = request.PrepTime,
                Servings = request.Servings,
                Image = request.Image,
                Enabled = false
            };

            await _recipeRepo.AddAsync(recipe);
            
            var ingredients = request.Ingredients.Select(item => new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                RecipeId = recipe.Id,
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                Unit = item.Unit
            }).ToList();

            await _recipeIngredientRepo.AddRangeAsync(ingredients);

            var steps = request.RecipeSteps.Select(item => new Step
            {
                Id = Guid.NewGuid(),
                RecipeId = recipe.Id,
                StepOrder = item.StepOrder,
                Instruction = item.Instruction,
                Image = item.ImageStep
            }).ToList();

            await _stepRepo.AddRangeAsync(steps);

            await _context.SaveChangesAsync();

            return recipe;
        }
    }
}