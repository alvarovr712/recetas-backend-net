using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.DTOs;
using RecetasAPINet.Security;


namespace RecetasAPINet.Services
{
    public class AuthService : IAuthService
    {
        private readonly RecetasDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(RecetasDbContext context, IPasswordHasher<User> passwordHasher, IJwtService iJwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = iJwtService;
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

        public async Task Logout()
        {
            await Task.CompletedTask;
        }

        public TokenInfoDTO TokenInfo(string token)
        {
            return _jwtService.ValidateToken(token);
        }
    }
}