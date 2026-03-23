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

        Task<PagedResponseDTO<UserDTO>> GetAllAsync(Guid requesterId, int page, int pageSize);

        Task<UserDTO> ToggleEnabledAsync(Guid requesterId, Guid targetUserId);

       
    }
}