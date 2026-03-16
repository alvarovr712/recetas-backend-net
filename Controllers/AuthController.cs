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
                var loginResponse = await _authService.Login(loginRequest);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("auth_token", loginResponse.Token, cookieOptions);
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
            return Ok(new{message ="Logout correcto"});
        }

        [HttpGet("me")]
        public async Task<IActionResult> TokenInfo()
        {
            var token = Request.Cookies["auth_token"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized("No hay token");
            try
            {
                var info = await _authService.TokenInfo(token);
                return Ok(info);
            }
            catch (Exception ex)
            {
                // Si hay error (sesión revocada, expirada, etc), limpiamos la cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(-1)
                };
                Response.Cookies.Append("auth_token", "", cookieOptions);
                
                return Unauthorized($"Sesión terminada: {ex.Message}");
            }
        }

    }
}