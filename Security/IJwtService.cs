using RecetasAPINet.Models;
using RecetasAPINet.DTOs;

namespace RecetasAPINet.Security
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        TokenInfoDTO ValidateToken(string token);
    }
}
