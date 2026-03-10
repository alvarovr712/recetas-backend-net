using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Services;

namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("recipe")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RecipeController: ControllerBase
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

            if(userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");
            
            Guid userId = Guid.Parse(userIdClaim.Value);

            var recipe = await _recipeService.CrearRecetaAsync(request,userId);

            return Ok(recipe);
            
        }

        [HttpGet("mis-recetas")]
        public async Task<ActionResult<IEnumerable<RecipeCardDto>>> GetMisRecetas()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if(userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            var userId = Guid.Parse(userIdClaim.Value);

            var recetas = await _recipeService.GetMisRecetasAsync(userId);
            return Ok(recetas);
        }
    }
}