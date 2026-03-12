using System.Security.Claims;
using RecetasAPINet.DTOs;
using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync (User user, IFormFile? imageFile);
        Task<UserProfileDTO> GetUserProfileAsync(ClaimsPrincipal userClaims);

        Task<User> UpdateUserAsync(Guid userId, UpdateUserDto dto);
       
    }
}