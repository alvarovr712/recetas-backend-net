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

        public async Task<TokenInfoDTO> TokenInfo(string token)
        {
            var info = _jwtService.ValidateToken(token);
            if (string.IsNullOrEmpty(info.UserId)) return info;

            // Fetch fresh data from DB to ensure sync (e.g. image change)
            if (Guid.TryParse(info.UserId, out var guid))
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == guid);
                if (user != null)
                {
                    info.Image = user.Image ?? string.Empty;
                    info.Username = user.Username;
                }
            }

            return info;
        }
    }
}