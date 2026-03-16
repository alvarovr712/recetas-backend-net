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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(RecetasDbContext context, IPasswordHasher<User> passwordHasher, IJwtService iJwtService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = iJwtService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<LoginResponse> Login(LoginRequest loginRequest)
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

            // Crear sesión en base de datos
            var httpContext = _httpContextAccessor.HttpContext;
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "unknown";
            
            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Ip = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown",
                Browser = ParseBrowser(userAgent),
                Enabled = true
            };

            // Generar el token incluyendo el ID de la sesión
            var token = _jwtService.GenerateToken(user, session.Id);
            session.Token = token; // Guardamos el token en la sesión

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return new LoginResponse 
            {
                User = user,
                Token = token
            };
        }

        private string ParseBrowser(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Desconocido";

            string browser = "Otro";
            string os = "Desconocido";

            // Detectar Navegador
            if (userAgent.Contains("Firefox")) browser = "Firefox";
            else if (userAgent.Contains("Chrome")) browser = "Chrome";
            else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) browser = "Safari";
            else if (userAgent.Contains("Edge")) browser = "Edge";

            // Detectar OS
            if (userAgent.Contains("Windows")) os = "Windows";
            else if (userAgent.Contains("Android")) os = "Android";
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) os = "iOS";
            else if (userAgent.Contains("Macintosh")) os = "macOS";
            else if (userAgent.Contains("Linux")) os = "Linux";

            return $"{browser} ({os})";
        }

        public async Task Logout()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token)) return;

            try 
            {
                var info = _jwtService.ValidateToken(token);
                if (Guid.TryParse(info.SessionId, out var sessionId))
                {
                    var session = await _context.Sessions.FindAsync(sessionId);
                    if (session != null)
                    {
                        session.Enabled = false;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch { /* Token ya inválido o expirado */ }
        }

        public async Task<TokenInfoDTO> TokenInfo(string token)
        {
            var info = _jwtService.ValidateToken(token);
            if (string.IsNullOrEmpty(info.UserId)) return info;

            // Validar sesión en DB
            if (Guid.TryParse(info.SessionId, out var sessionId))
            {
                var session = await _context.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
                if (session == null || !session.Enabled || session.ExpiresAt < DateTime.UtcNow)
                {
                    throw new Exception("Sesión inválida o expirada");
                }
            }
            else 
            {
                throw new Exception("Token no contiene identificador de sesión");
            }

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

        public async Task<bool> ValidateSessionAsync(Guid sessionId)
        {
            var session = await _context.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
            return session != null && session.Enabled && session.ExpiresAt > DateTime.UtcNow;
        }
    }
}