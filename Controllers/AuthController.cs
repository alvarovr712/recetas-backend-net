using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.DTOs;
using System.ComponentModel.DataAnnotations;
using RecetasAPINet.Enums;
using RecetasAPINet.Security;


namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route ("auth")]
    public class AuthController : ControllerBase
    {
         private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController (IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _authService.Login(loginRequest);

                var token = _jwtService.GenerateToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                Response.Cookies.Append("auth_token" , token, cookieOptions);
                return Ok ("Login correcto");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}