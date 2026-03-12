
using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.DTOs;
using System.ComponentModel.DataAnnotations;
using RecetasAPINet.Enums;
using RecetasAPINet.Security;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await iUserService.GetUserProfileAsync(User);
            return Ok(profile);
        }

        
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var updatedUser = await iUserService.UpdateUserAsync(userId, dto);
            return Ok(updatedUser);
        }




    }
}