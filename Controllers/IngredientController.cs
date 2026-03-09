using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Services;

namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("ingredients")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _service;

        public IngredientController(IIngredientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _service.BuscarTodos();
            return Ok(ingredients);
        }
        [HttpPost("crear")]
        public async Task<IActionResult> CrearIngrediente([FromBody] Dictionary<string, string> data)
        {
            string nombre = data["nombre"];
            var ingrediente = await _service.CrearIngrediente(nombre);
            return Ok(ingrediente);
        }
    }
}