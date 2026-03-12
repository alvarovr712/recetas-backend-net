using Microsoft.AspNetCore.Http;

namespace RecetasAPINet.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surnames { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }
}
