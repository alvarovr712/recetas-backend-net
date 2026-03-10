using Microsoft.Extensions.Configuration.UserSecrets;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.Repositories;

using RecetasAPINet.Enums;

namespace RecetasAPINet.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecetasDbContext _context;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRecipeIngredientRepository _recipeIngredientRepo;
        private readonly IStepRepository _stepRepo;

        private readonly IUserRepository _userRepo;

        public RecipeService(RecetasDbContext context, IRecipeRepository recipeRepo, IRecipeIngredientRepository recipeIngredientRepo,
        IStepRepository stepRepo, IUserRepository userRepo)
        {
            _context = context;
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _stepRepo = stepRepo;
            _userRepo = userRepo;

        }

        public async Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request, Guid userId)
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                Type = Enum.TryParse<RecipeType>(request.Type, true, out var recipeType) ? recipeType : RecipeType.Principal,
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

        public async Task<List<RecipeCardDto>> GetMisRecetasAsync(Guid userId)
        {
            return await _recipeRepo.GetRecipesByUserIdAsync(userId);
        }

        public async Task<RecipeDetailDto?> GetRecipeDetailAsync(Guid recipeId)
        {
            // 1. Buscar la receta
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            var user = await _userRepo.GetByIdAsync(recipe.UserId);

            if (recipe == null)
                return null;

            // 2. Construir el DTO SOLO con los datos de la receta
            var dto = new RecipeDetailDto
            {
                Id = recipe.Id,
                Image = recipe.Image ?? string.Empty,
                Title = recipe.Title ?? string.Empty,
                Type = recipe.Type, // enum RecipeType
                PrepTime = recipe.PrepTime,
                Servings = recipe.Servings,
                Description = recipe.Description ?? string.Empty,


                UserName = user?.Username ?? string.Empty,
                UserImage = user?.Image ?? string.Empty,




                // estos se rellenarán en el paso 3 y 4
                Ingredients = new List<RecipeIngredientDto>(),
                Steps = new List<RecipeStepDto>()
            };

            var ingredients = await _recipeIngredientRepo.GetByRecipeIdAsync(recipeId);

            dto.Ingredients = ingredients
                .Select(i => new RecipeIngredientDto
                {
                    Quantity = i.Quantity.ToString(),
                    Unit = i.Unit ?? string.Empty
                })
                .ToList();

            var steps = await _stepRepo.GetByRecipeIdAsync(recipeId);

            dto.Steps = steps
                .OrderBy(s => s.StepOrder)
                .Select(s => new RecipeStepDto
                {
                    StepOrder = s.StepOrder,
                    Instruction = s.Instruction ?? string.Empty,
                    Image = s.Image ?? string.Empty
                })
                .ToList();

            return dto;
        }

    }
}