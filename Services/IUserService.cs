using RecetasAPINet.DTOs;
using RecetasAPINet.Models;

namespace RecetasAPINet.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync (User user, IFormFile? imageFile);
       
    }
}