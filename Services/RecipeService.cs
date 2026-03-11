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

        private readonly IUserFavoriteRepository _userFavoriteRepo;

        public RecipeService(RecetasDbContext context, IRecipeRepository recipeRepo, IRecipeIngredientRepository recipeIngredientRepo,
        IStepRepository stepRepo, IUserRepository userRepo, IUserFavoriteRepository userFavoriteRepo)
        {
            _context = context;
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _stepRepo = stepRepo;
            _userRepo = userRepo;
            _userFavoriteRepo = userFavoriteRepo;

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
                Enabled = true
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
            var recetas = await _recipeRepo.GetRecipesByUserIdAsync(userId);

            return recetas
                .Where(r => r.Enabled)
                .Select(r => new RecipeCardDto
                {
                    Id = r.Id,
                    Image = r.Image,
                    Title = r.Title,
                    Description = r.Description,
                    Type = r.Type,
                    IsFavorite = _context.UserFavorites
                        .Any(f => f.UserId == userId && f.RecipeId == r.Id)
                })
                .ToList();
        }



        public async Task<RecipeDetailDto?> GetRecipeDetailAsync(Guid recipeId, Guid userId)
        {
            // 1. Buscar la receta
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);


            if (recipe == null)
                return null;

            var user = await _userRepo.GetByIdAsync(recipe.UserId);

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


                Ingredients = new List<RecipeIngredientDto>(),
                Steps = new List<RecipeStepDto>()
            };

            var ingredients = await _recipeIngredientRepo.GetByRecipeIdAsync(recipeId);

            dto.Ingredients = ingredients
                .Select(i => new RecipeIngredientDto
                {
                    Name = i.Ingredient?.Name ?? string.Empty,
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

            dto.IsFavorite = await _userFavoriteRepo.GetAsync(userId, recipeId) != null;

            return dto;
        }

        public async Task<List<RecipeCardDto>> GetAllRecetasAsync(Guid userId, string? category = null)
        {
            var recetas = await _recipeRepo.GetAll();

            // 1. Solo recetas habilitadas
            recetas = recetas.Where(r => r.Enabled).ToList();

            // 2. Filtrar por categoría si viene
            if (!string.IsNullOrEmpty(category) && category != "Todo")
            {
                if (Enum.TryParse<RecipeType>(category, out var typeEnum))
                {
                    recetas = recetas.Where(r => r.Type == typeEnum).ToList();
                }
            }

            // 3. Mapear a DTO
            return recetas
                .Select(r => new RecipeCardDto
                {
                    Id = r.Id,
                    Image = r.Image ?? string.Empty,
                    Title = r.Title ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    Type = r.Type,
                    IsFavorite = _context.UserFavorites
                        .Any(f => f.UserId == userId && f.RecipeId == r.Id)
                })
                .ToList();
        }




        public async Task<bool> ToggleFavoriteAsync(Guid userId, Guid recipeId)
        {
            var existing = await _userFavoriteRepo.GetAsync(userId, recipeId);

            if (existing != null)
            {
                await _userFavoriteRepo.RemoveAsync(existing);
                await _userFavoriteRepo.SaveChangesAsync();
                return false;
            }

            var newFav = new UserFavorite
            {
                UserId = userId,
                RecipeId = recipeId
            };

            await _userFavoriteRepo.AddAsync(newFav);
            await _userFavoriteRepo.SaveChangesAsync();
            return true;
        }

        public async Task<List<RecipeCardDto>> GetFavoritasAsync(Guid userId)
        {
            // 1. Obtener favoritos del usuario
            var favoritos = await _userFavoriteRepo.GetByUserIdAsync(userId);

            // Si no tiene favoritos, devolvemos lista vacía
            if (!favoritos.Any())
                return new List<RecipeCardDto>();

            // 2. Obtener los IDs de recetas favoritas
            var recipeIds = favoritos.Select(f => f.RecipeId).ToList();

            // 3. Obtener las recetas correspondientes
            var recetas = await _recipeRepo.GetByIdsAsync(recipeIds);

            // 4. Filtrar solo las habilitadas
            recetas = recetas.Where(r => r.Enabled).ToList();

            // 5. Construir DTOs
            return recetas
                .Select(r => new RecipeCardDto
                {
                    Id = r.Id,
                    Image = r.Image ?? string.Empty,
                    Title = r.Title ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    Type = r.Type,
                    IsFavorite = true
                })
                .ToList();
        }



    }
}