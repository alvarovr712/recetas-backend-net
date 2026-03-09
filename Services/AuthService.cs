using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.DTOs;


namespace RecetasAPINet.Services
{
    public class AuthService : IAuthService
    {
        private readonly RecetasDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(RecetasDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
         public async Task<User> Login(LoginRequest loginRequest)
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

        }
    }
}