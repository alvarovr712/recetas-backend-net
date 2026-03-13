using Microsoft.Extensions.Configuration.UserSecrets;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.Repositories;

using RecetasAPINet.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using RecetasAPINet.DTOs;

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

        private readonly IImageService _imageService;



        public RecipeService(RecetasDbContext context, IRecipeRepository recipeRepo, IRecipeIngredientRepository recipeIngredientRepo,
        IStepRepository stepRepo, IUserRepository userRepo, IUserFavoriteRepository userFavoriteRepo, IIngredientRepository ingredientRepo,
        ILogRepository logRepository, IImageService imageService)
        {
            _context = context;
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _stepRepo = stepRepo;
            _userRepo = userRepo;
            _userFavoriteRepo = userFavoriteRepo;
            _ingredientRepo = ingredientRepo;
            _logRepository = logRepository;
            _imageService = imageService;

        }

        public async Task<Recipe> CrearRecetaAsync(CreateRecipeRequest request, Guid userId)
        {

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
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

            var steps = request.Steps.Select(item => new Step
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
                    IngredientId = i.IngredientId,
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

        public async Task<Recipe> EditarRecetaAsync(UpdateRecipeDTO request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Id) || !Guid.TryParse(request.Id, out var recipeId))
                throw new Exception($"El ID de la receta '{request.Id}' no es un GUID válido.");

            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new Exception($"No se encontró la receta con ID {recipeId}");

            if (recipe.UserId != userId)
                throw new Exception("No tienes permiso para editar esta receta");

            string? oldImageUrl = recipe.Image;
            string? newImageUrl = null;

            try
            {
                // ----------------------------------------------------
                // 1. Campos básicos
                // ----------------------------------------------------
                if (!string.IsNullOrWhiteSpace(request.Title))
                    recipe.Title = request.Title;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    recipe.Description = request.Description;

                if (!string.IsNullOrWhiteSpace(request.Type) &&
                    Enum.TryParse<RecipeType>(request.Type, true, out var parsedType))
                {
                    recipe.Type = parsedType;
                }

                if (request.PrepTime.HasValue)
                    recipe.PrepTime = request.PrepTime.Value;

                if (request.Servings.HasValue)
                    recipe.Servings = request.Servings.Value;

                // ----------------------------------------------------
                // 2. Imagen principal
                if (!string.IsNullOrWhiteSpace(request.Image))
                {
                    recipe.Image = request.Image;
                }

                recipe.UpdatedAt = DateTime.UtcNow;

                // ----------------------------------------------------
                // 3. Ingredientes (reemplazar todos)
                // ----------------------------------------------------
                if (request.Ingredients != null)
                {
                    await _recipeIngredientRepo.DeleteByRecipeIdAsync(recipe.Id);

                    var newIngredients = new List<RecipeIngredient>();
                    foreach (var ingDto in request.Ingredients)
                    {
                        if (Guid.TryParse(ingDto.IngredientId, out var ingGuid))
                        {
                            // Convertir cantidad a double de forma segura (usando punto como separador decimal)
                            double.TryParse(ingDto.Quantity?.Replace(',', '.'), 
                                System.Globalization.NumberStyles.Any, 
                                System.Globalization.CultureInfo.InvariantCulture, 
                                out var quantity);

                            newIngredients.Add(new RecipeIngredient
                            {
                                Id = Guid.NewGuid(),
                                RecipeId = recipe.Id,
                                IngredientId = ingGuid,
                                Quantity = quantity,
                                Unit = ingDto.Unit ?? string.Empty
                            });
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(ingDto.IngredientId))
                                throw new Exception($"El ID del ingrediente '{ingDto.IngredientId}' no es un GUID válido.");
                        }
                    }

                    if (newIngredients.Any())
                        await _recipeIngredientRepo.AddRangeAsync(newIngredients);
                }

                // ----------------------------------------------------
                // 4. Pasos (reemplazar todos)
                // ----------------------------------------------------
                if (request.Steps != null)
                {
                    // 4.1 Obtener pasos antiguos
                    var oldSteps = await _stepRepo.GetByRecipeIdAsync(recipe.Id);

                    // 4.2 Calcular qué imágenes de pasos NO se reutilizan en los nuevos pasos
                    var newStepImages = request.Steps
                        .Where(s => !string.IsNullOrEmpty(s.ImageStep))
                        .Select(s => s.ImageStep!)
                        .ToHashSet();

                    foreach (var step in oldSteps)
                    {
                        if (!string.IsNullOrEmpty(step.Image) && !newStepImages.Contains(step.Image))
                            _imageService.DeleteImage(step.Image);
                    }

                    // 4.3 Borrar pasos antiguos
                    await _stepRepo.DeleteByRecipeIdAsync(recipe.Id);

                    // 4.4 Insertar nuevos pasos
                    var newSteps = request.Steps
                        .Where(s => !string.IsNullOrWhiteSpace(s.Instruction))
                        .Select((s, index) => new Step
                        {
                            Id = Guid.NewGuid(),
                            RecipeId = recipe.Id,
                            StepOrder = s.StepOrder ?? (index + 1),
                            Instruction = s.Instruction ?? string.Empty,
                            Image = s.ImageStep
                        }).ToList();

                    if (newSteps.Any())
                        await _stepRepo.AddRangeAsync(newSteps);
                }

                // ----------------------------------------------------
                // 5. Guardar cambios de la receta
                // ----------------------------------------------------
                await _recipeRepo.UpdateAsync(recipe);

                // ----------------------------------------------------
                // 6. Borrar imagen principal antigua si se subió imagen nueva
                // ----------------------------------------------------
                if (!string.IsNullOrEmpty(oldImageUrl) && recipe.Image != oldImageUrl)
                {
                    _imageService.DeleteImage(oldImageUrl);
                }

                return recipe;
            }
            catch
            {
                // Si falló algo después de subir la nueva imagen → borrarla
                if (newImageUrl != null)
                    _imageService.DeleteImage(newImageUrl);

                throw;
            }
        }

        public async Task<bool> EliminarRecetaAsync(Guid recipeId, Guid userId)
        {
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new Exception("Receta no encontrada");

            if (recipe.UserId != userId)
                throw new Exception("No tienes permiso para eliminar esta receta");

            // 1. Borrar pasos e imágenes de los pasos
            var steps = await _stepRepo.GetByRecipeIdAsync(recipeId);
            foreach (var step in steps)
            {
                if (!string.IsNullOrEmpty(step.Image))
                    _imageService.DeleteImage(step.Image);
            }
            await _stepRepo.DeleteByRecipeIdAsync(recipeId);

            // 2. Borrar ingredientes
            await _recipeIngredientRepo.DeleteByRecipeIdAsync(recipeId);

            // 3. Borrar imagen principal
            if (!string.IsNullOrEmpty(recipe.Image))
                _imageService.DeleteImage(recipe.Image);

            // 4. Borrar receta
            await _recipeRepo.DeleteAsync(recipe);

            return true;
        }

    }
}