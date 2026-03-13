using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.DTOs;
using RecetasAPINet.Services;

namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("recipe")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearReceta([FromBody] CreateRecipeRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recipe = await _recipeService.CrearRecetaAsync(request, userId);

            return Ok(recipe);

        }
        [HttpGet("mis-recetas")]
        public async Task<ActionResult<IEnumerable<RecipeCardDto>>> GetMisRecetas([FromQuery] string? category)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.GetMisRecetasAsync(userId, category);
            return Ok(recetas);
        }




        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<RecipeDetailDto>> GetRecipeDetail(Guid id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
            Guid userId = Guid.Empty;

            if (userIdClaim != null)
            {
                userId = Guid.Parse(userIdClaim.Value);
            }

            var result = await _recipeService.GetRecipeDetailAsync(id, userId);

            if (result == null)
                return NotFound("Receta no encontrada");

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<RecipeCardDto>>> GetAll([FromQuery] string? category)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.GetAllRecetasAsync(userId, category);
            return Ok(recetas);
        }



        [HttpPost("toggle-favorite/{recipeId}")]
        public async Task<ActionResult> ToggleFavorite(Guid recipeId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var isFavorite = await _recipeService.ToggleFavoriteAsync(userId, recipeId);

            return Ok(new { favorite = isFavorite });
        }

        [HttpGet("favoritas")]
        public async Task<ActionResult<List<RecipeCardDto>>> GetFavoritas([FromQuery] string? category)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.GetFavoritasAsync(userId, category);
            return Ok(recetas);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<List<RecipeCardDto>>> Buscar([FromQuery] string filtro)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.FiltroByTitleDescriptionOrIngredientAsync(userId, filtro);

            return Ok(recetas);
        }

        [HttpGet("mis-recetas/buscar")]
        public async Task<ActionResult<List<RecipeCardDto>>> BuscarMisRecetas([FromQuery] string filtro)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.FiltroMisRecetasAsync(userId, filtro);

            return Ok(recetas);
        }

        [HttpGet("favoritas/buscar")]
        public async Task<ActionResult<List<RecipeCardDto>>> BuscarFavoritas([FromQuery] string filtro)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.FiltroFavoritasAsync(userId, filtro);

            return Ok(recetas);
        }

        [HttpPut("editar")]
        public async Task<IActionResult> EditarReceta([FromBody] UpdateRecipeDTO dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                               ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            try
            {
                var updatedRecipe = await _recipeService.EditarRecetaAsync(dto, userId);
                return Ok(updatedRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarReceta(Guid id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                               ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            try
            {
                await _recipeService.EliminarRecetaAsync(id, userId);
                return Ok(new { message = "Receta eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}