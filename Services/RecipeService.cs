using Microsoft.Extensions.Configuration.UserSecrets;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.Repositories;

using RecetasAPINet.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

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

        private readonly IIngredientRepository _ingredientRepo;

        private readonly ILogRepository _logRepository;

        public RecipeService(RecetasDbContext context, IRecipeRepository recipeRepo, IRecipeIngredientRepository recipeIngredientRepo,
        IStepRepository stepRepo, IUserRepository userRepo, IUserFavoriteRepository userFavoriteRepo, IIngredientRepository ingredientRepo,
        ILogRepository logRepository)
        {
            _context = context;
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _stepRepo = stepRepo;
            _userRepo = userRepo;
            _userFavoriteRepo = userFavoriteRepo;
            _ingredientRepo = ingredientRepo;
            _logRepository = logRepository;

        }

        public async Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request, Guid userId)
        {

            var user = await _userRepo.GetByIdAsync(userId);
            if(user == null)
                throw new Exception("Usuario no encontrado");

            await _logRepository.AddAsync(new Log
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = "CrearReceta",
                Description = $"El usuario {user.Username} creó la receta: {request.Title}",
                CreatedAt = DateTime.UtcNow
            });

            await _logRepository.SaveChangesAsync();
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
                CreatedAt = DateTime.UtcNow,
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

        public async Task<List<RecipeCardDto>> GetMisRecetasAsync(Guid userId, string? category = null)
        {
            var recetas = await _recipeRepo.GetRecipesByUserIdAsync(userId);

            // 1. Solo habilitadas
            recetas = recetas.Where(r => r.Enabled).ToList();

            // 2. Filtrar por categoría si viene
            if (!string.IsNullOrEmpty(category) && category != "Todo")
            {
                if (Enum.TryParse<RecipeType>(category, out var typeEnum))
                {
                    recetas = recetas.Where(r => r.Type == typeEnum).ToList();
                }
            }


            return recetas
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

        public async Task<List<RecipeCardDto>> GetFavoritasAsync(Guid userId, string? category = null)
        {
            // 1. Obtener favoritos del usuario
            var favoritos = await _userFavoriteRepo.GetByUserIdAsync(userId);

            if (!favoritos.Any())
                return new List<RecipeCardDto>();

            // 2. IDs de recetas favoritas
            var recipeIds = favoritos.Select(f => f.RecipeId).ToList();

            // 3. Obtener recetas
            var recetas = await _recipeRepo.GetByIdsAsync(recipeIds);

            // 4. Solo habilitadas
            recetas = recetas.Where(r => r.Enabled).ToList();

            // 5. Filtrar por categoría si viene
            if (!string.IsNullOrEmpty(category) && category != "Todo")
            {
                if (Enum.TryParse<RecipeType>(category, out var typeEnum))
                {
                    recetas = recetas.Where(r => r.Type == typeEnum).ToList();
                }
            }

            // 6. Mapear DTOs
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

        public async Task<List<RecipeCardDto>> FiltroByTitleDescriptionOrIngredientAsync(Guid userId, string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return new List<RecipeCardDto>();

            filtro = filtro.ToLower().Trim();

            // ---------------------------------------------------------
            // 1. Buscar por título o descripción
            // ---------------------------------------------------------
            var recetasTexto = await _recipeRepo.searchByTitleOrDescriptionAsync(filtro);

            // ---------------------------------------------------------
            // 2. Buscar ingredientes por nombre
            // ---------------------------------------------------------
            var ingredientes = await _ingredientRepo.SearchIngredientsByNameAsync(filtro);

            List<Recipe> recetasIngredientes = new();

            if (ingredientes.Any())
            {
                // Obtener IDs de ingredientes
                var ingredientIds = ingredientes.Select(i => i.Id).ToList();

                // Buscar en RecipeIngredient por esos ingredientIds
                var recipeIngredients = await _recipeIngredientRepo.GetByIngredientIdsAsync(ingredientIds);

                if (recipeIngredients.Any())
                {
                    // Obtener los RecipeId únicos
                    var recipeIds = recipeIngredients
                        .Select(ri => ri.RecipeId)
                        .Distinct()
                        .ToList();

                    // Recuperar las recetas asociadas
                    recetasIngredientes = await _recipeRepo.GetByIdsAsync(recipeIds);
                }
            }

            // ---------------------------------------------------------
            // 3. Unir resultados sin duplicados
            // ---------------------------------------------------------
            var recetasFinal = recetasTexto
                .Union(recetasIngredientes)
                .Where(r => r.Enabled) // solo habilitadas
                .Distinct()
                .ToList();

            // ---------------------------------------------------------
            // 4. Mapear a DTO
            // ---------------------------------------------------------
            var favoritos = _context.UserFavorites
                .Where(f => f.UserId == userId)
                .Select(f => f.RecipeId)
                .ToHashSet();

            return recetasFinal
                .Select(r => new RecipeCardDto
                {
                    Id = r.Id,
                    Image = r.Image ?? string.Empty,
                    Title = r.Title ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    Type = r.Type,
                    IsFavorite = favoritos.Contains(r.Id)
                })
                .ToList();
        }

        public async Task<List<RecipeCardDto>> FiltroMisRecetasAsync(Guid userId, string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return new List<RecipeCardDto>();

            filtro = filtro.ToLower().Trim();

            // ---------------------------------------------------------
            // 1. Buscar mis recetas por título o descripción
            // ---------------------------------------------------------
            var recetasTexto = await _recipeRepo.searchByTitleOrDescriptionAsync(filtro);

            // Filtrar solo las mías
            recetasTexto = recetasTexto
                .Where(r => r.UserId == userId)
                .ToList();

            // ---------------------------------------------------------
            // 2. Buscar ingredientes por nombre
            // ---------------------------------------------------------
            var ingredientes = await _ingredientRepo.SearchIngredientsByNameAsync(filtro);

            List<Recipe> recetasIngredientes = new();

            if (ingredientes.Any())
            {
                var ingredientIds = ingredientes.Select(i => i.Id).ToList();

                var recipeIngredients = await _recipeIngredientRepo.GetByIngredientIdsAsync(ingredientIds);

                if (recipeIngredients.Any())
                {
                    var recipeIds = recipeIngredients
                        .Select(ri => ri.RecipeId)
                        .Distinct()
                        .ToList();

                    // Recuperar recetas asociadas
                    recetasIngredientes = await _recipeRepo.GetByIdsAsync(recipeIds);

                    // Filtrar solo las mías
                    recetasIngredientes = recetasIngredientes
                        .Where(r => r.UserId == userId)
                        .ToList();
                }
            }

            // ---------------------------------------------------------
            // 3. Unir resultados sin duplicados
            // ---------------------------------------------------------
            var recetasFinal = recetasTexto
                .Union(recetasIngredientes)
                .Where(r => r.Enabled)
                .Distinct()
                .ToList();

            // ---------------------------------------------------------
            // 4. Mapear a DTO
            // ---------------------------------------------------------
            return recetasFinal
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

        public async Task<List<RecipeCardDto>> FiltroFavoritasAsync(Guid userId, string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return new List<RecipeCardDto>();

            filtro = filtro.ToLower().Trim();

            // ---------------------------------------------------------
            // 1. Obtener IDs de recetas favoritas del usuario
            // ---------------------------------------------------------
            var favoriteIds = await _userFavoriteRepo.GetByUserIdAsync(userId);
            var favRecipeIds = favoriteIds.Select(f => f.RecipeId).ToList();

            if (!favRecipeIds.Any())
                return new List<RecipeCardDto>();

            // ---------------------------------------------------------
            // 2. Buscar por título o descripción dentro de favoritas
            // ---------------------------------------------------------
            var recetasTexto = await _recipeRepo.searchByTitleOrDescriptionAsync(filtro);
            recetasTexto = recetasTexto
                .Where(r => favRecipeIds.Contains(r.Id))
                .ToList();

            // ---------------------------------------------------------
            // 3. Buscar ingredientes por nombre
            // ---------------------------------------------------------
            var ingredientes = await _ingredientRepo.SearchIngredientsByNameAsync(filtro);

            List<Recipe> recetasIngredientes = new();

            if (ingredientes.Any())
            {
                var ingredientIds = ingredientes.Select(i => i.Id).ToList();

                var recipeIngredients = await _recipeIngredientRepo.GetByIngredientIdsAsync(ingredientIds);

                if (recipeIngredients.Any())
                {
                    var recipeIds = recipeIngredients
                        .Select(ri => ri.RecipeId)
                        .Distinct()
                        .ToList();

                    // Recuperar recetas asociadas
                    recetasIngredientes = await _recipeRepo.GetByIdsAsync(recipeIds);

                    // Filtrar solo favoritas
                    recetasIngredientes = recetasIngredientes
                        .Where(r => favRecipeIds.Contains(r.Id))
                        .ToList();
                }
            }

            // ---------------------------------------------------------
            // 4. Unir resultados sin duplicados
            // ---------------------------------------------------------
            var recetasFinal = recetasTexto
                .Union(recetasIngredientes)
                .Where(r => r.Enabled)
                .Distinct()
                .ToList();

            // ---------------------------------------------------------
            // 5. Mapear a DTO
            // ---------------------------------------------------------
            return recetasFinal
                .Select(r => new RecipeCardDto
                {
                    Id = r.Id,
                    Image = r.Image ?? string.Empty,
                    Title = r.Title ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    Type = r.Type,
                    IsFavorite = true // todas son favoritas
                })
                .ToList();
        }







    }
}