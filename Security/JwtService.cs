using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecetasAPINet.Models;
using RecetasAPINet.Enums;
using RecetasAPINet.DTOs;

namespace RecetasAPINet.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("role", user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public TokenInfoDTO ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Key);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                            
            var username = principal.FindFirst("username")?.Value 
                        ?? principal.FindFirst(ClaimTypes.Name)?.Value;
                        
            var roleString = principal.FindFirst("role")?.Value 
                          ?? principal.FindFirst(ClaimTypes.Role)?.Value;

            return new TokenInfoDTO
            {
                UserId = int.TryParse(userIdString, out var uid) ? uid : 0,
                Username = username ?? string.Empty,
                Role = Enum.TryParse<Role>(roleString, true, out var role) ? role : Role.User,
                Expires = validatedToken.ValidTo
            };
        }
    }
}
