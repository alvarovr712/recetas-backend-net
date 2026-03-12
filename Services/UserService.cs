using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.DTOs;
using RecetasAPINet.Repositories;

namespace RecetasAPINet.Services
{
    public class UserService : IUserService
    {
        private readonly RecetasDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly ILogRepository _logRepository;

        public UserService(RecetasDbContext context, IPasswordHasher<User> passwordHasher, ILogRepository logRepository)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logRepository = logRepository;
        }



        public async Task<User> CreateUserAsync(User user, IFormFile? imageFile)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
                throw new Exception("El email ya está registrado");

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (usernameExists)
                throw new Exception("El username ya está registrado");

            user.Role = Enums.Role.User;
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.Enabled = true;

            var folderPath = Path.Combine("wwwroot", "ImageUsers");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (imageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                user.Image = $"/ImageUsers/{fileName}";
            }


            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

             await _logRepository.AddAsync(new Log
            {
                Id = Guid.NewGuid(),
                UserId = user.Id, 
                Action = "CrearUsuario",
                Description = $"Se creo un nuevo usuario con username '{user.Username}' y email '{user.Email}'",
                CreatedAt = DateTime.UtcNow
            });

            await _logRepository.SaveChangesAsync();

            return user;
        }
    }
}
