using RecetasAPINet.Models;

namespace RecetasAPINet.Security
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
