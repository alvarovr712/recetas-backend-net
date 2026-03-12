using RecetasAPINet.DTOs;
using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IAuthService
    {
        Task<User> Login(LoginRequest loginRequest);
        Task Logout();
        Task<TokenInfoDTO> TokenInfo(string token);
    }
}