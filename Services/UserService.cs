using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.DTOs;

namespace RecetasAPINet.Services
{
    public class UserService : IUserService
    {
        private readonly RecetasDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(RecetasDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (emailExists)
            {
                throw new Exception("El email  ya está registrado");
            }

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == user.Username);

            if (usernameExists)
            {
                throw new Exception("El username ya está registrado");
            }

            user.Role = Enums.Role.User;

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            user.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

       /* public async Task<User> Login(LoginRequest loginRequest)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Identifier || u.Username == loginRequest.Identifier);

            if(user == null)
            {
                throw new Exception("Email o Username incorrecto");

            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Contraseña incorrecta");
            }

            return user;

        }*/
    }
}