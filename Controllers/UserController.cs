
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
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService iUserService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService iuserService, IJwtService jwtService)
        {
            iUserService = iuserService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromForm] RegisterDto dto)
        {
            try
            {
                var user = new User
                {
                    Name = dto.Name,
                    Surnames = dto.Surnames,
                    Email = dto.Email,
                    Username = dto.Username,
                    Password = dto.Password
                };

                var created = await iUserService.CreateUserAsync(user, dto.Image);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        /*  [HttpPost("login")]
          public async Task<IActionResult> Login ([FromBody] LoginRequest loginRequest)
          {
              try
              {
                  var user = await iUserService.Login(loginRequest);

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
          }*/

    }
}