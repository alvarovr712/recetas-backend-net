using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Services;

namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("images")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var url = await _imageService.SaveImageAsync(file);
            return Ok(new { url });
        }

        [HttpDelete("delete")]
        public IActionResult DeleteImage([FromQuery] string url)
        {
            _imageService.DeleteImage(url);
            return Ok();
        }
    }
}