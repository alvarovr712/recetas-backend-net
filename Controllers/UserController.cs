
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            // 1) Obtener ID del usuario desde el JWT
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                               ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            try
            {
                // 2) Pasar el userId al servicio
                var pagedResponse = await iUserService.GetAllAsync(userId, page, pageSize);
                return Ok(pagedResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPut("toggle-enabled/{id}")]
        public async Task<IActionResult> ToggleEnabled(Guid id)
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                               ?? User.FindFirst("id");

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el ID del usuario del token");

            Guid requesterId = Guid.Parse(userIdClaim.Value);

            try
            {
              
                var updatedUser = await iUserService.ToggleEnabledAsync(requesterId,id);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }







    }
}