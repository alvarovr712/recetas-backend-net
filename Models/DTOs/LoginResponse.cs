using RecetasAPINet.Models;

namespace RecetasAPINet.DTOs
{
    public class LoginResponse
    {
        public User User { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
    }
}
