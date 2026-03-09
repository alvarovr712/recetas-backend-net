using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.DTOs;
using System.ComponentModel.DataAnnotations;
using RecetasAPINet.Enums;
using RecetasAPINet.Security;
using Microsoft.AspNetCore.Http.HttpResults;


namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
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

                Response.Cookies.Append("auth_token", token, cookieOptions);
                return Ok("Login correcto");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append("auth_token", "", cookieOptions);
            return Ok("Logout correcto");
        }

        [HttpGet("me")]
        public IActionResult TokenInfo()
        {
            var token = Request.Cookies["auth_token"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized("No hay token");
            try
            {

                var info = _authService.TokenInfo(token);
                return Ok(info);

            }
            catch (Exception ex)
            {
                return Unauthorized($"Token Inválido: {ex.Message}");
            }
        }

    }
}