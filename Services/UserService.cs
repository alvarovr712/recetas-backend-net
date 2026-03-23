using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.DTOs;
using RecetasAPINet.Repositories;
using System.Security.Claims;
using RecetasAPINet.Enums;

namespace RecetasAPINet.Services
{
    public class UserService : IUserService
    {
        private readonly RecetasDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        private readonly ILogRepository _logRepository;

        private readonly IUserRepository _userRepository;

        public UserService(RecetasDbContext context, IPasswordHasher<User> passwordHasher, ILogRepository logRepository,
        IUserRepository userRepository, CloudinaryDotNet.Cloudinary cloudinary)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logRepository = logRepository;
            _userRepository = userRepository;
            _cloudinary = cloudinary;
        }



        public async Task<User> CreateUserAsync(User user, IFormFile? imageFile)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
                throw new Exception("El email ya está registrado");

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (usernameExists)
                throw new Exception("El username ya está registrado");

            user.Role = Enums.Role.User;
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.Enabled = true;

            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
                {
                    File = new CloudinaryDotNet.FileDescription(imageFile.FileName, stream),
                    Folder = "ImageUsers"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception(uploadResult.Error.Message);

                user.Image = uploadResult.SecureUrl.ToString();
            }


            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _logRepository.AddAsync(new Log
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Action = "CrearUsuario",
                Description = $"Se creo un nuevo usuario con username '{user.Username}' y email '{user.Email}'",
                CreatedAt = DateTime.UtcNow
            });

            await _logRepository.SaveChangesAsync();

            return user;
        }

        public async Task<UserProfileDTO> GetUserProfileAsync(ClaimsPrincipal userClaims)
        {
            // 1. Obtener userId del token
            var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier)
                               ?? userClaims.FindFirst("sub")
                               ?? userClaims.FindFirst("id");

            if (userIdClaim == null)
                throw new Exception("No se pudo obtener el ID del usuario del token");

            Guid userId = Guid.Parse(userIdClaim.Value);

            // 2. Buscar usuario
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            // 3. Contar recetas creadas desde logs
            var recipesCreated = await _logRepository.CountRecipeCreationsAsync(userId);

            // 4. Devolver DTO
            return new UserProfileDTO
            {
                Name = user.Name,
                Surnames = user.Surnames,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                Image = user.Image,
                Role = user.Role.ToString(),
                RecipesCreated = recipesCreated
            };
        }

        public async Task<User> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            // Actualizar solo los campos enviados
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Surnames))
                user.Surnames = dto.Surnames;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = _passwordHasher.HashPassword(user, dto.Password);

            // Imagen opcional
            if (dto.Image != null)
            {
                // 1. Borrar imagen anterior si existe
                if (!string.IsNullOrEmpty(user.Image) && user.Image.Contains("cloudinary"))
                {
                    try 
                    {
                        var uri = new Uri(user.Image);
                        var segments = uri.Segments;
                        var fileNameWithExtension = segments.Last();
                        var publicId = Path.GetFileNameWithoutExtension(fileNameWithExtension);
                        var deletionParams = new CloudinaryDotNet.Actions.DeletionParams($"ImageUsers/{publicId}");
                        _cloudinary.Destroy(deletionParams);
                    } catch {}
                }

                // 2. Guardar la nueva imagen
                using var stream = dto.Image.OpenReadStream();
                var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
                {
                    File = new CloudinaryDotNet.FileDescription(dto.Image.FileName, stream),
                    Folder = "ImageUsers"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception(uploadResult.Error.Message);

                user.Image = uploadResult.SecureUrl.ToString();
            }

            // Guardar cambios usando el repositorio
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<PagedResponseDTO<UserDTO>> GetAllAsync(Guid requesterId, int page, int pageSize)
        {
            // 1) Buscar al usuario que hace la petición
            var requester = await _userRepository.GetByIdAsync(requesterId);

            if (requester == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // 2) Validar rol
            if (requester.Role != Role.Admin)
                throw new UnauthorizedAccessException("Acceso denegado. Se requiere rol Admin.");

            // 3) Obtener todos los usuarios con paginación
            var query = _context.Users.AsQueryable();
            var totalCount = await query.CountAsync();

            List<User> users;
            if (pageSize == -1) // "Todos"
            {
                users = await query.ToListAsync();
                pageSize = totalCount > 0 ? totalCount : 1;
            }
            else 
            {
                users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return new PagedResponseDTO<UserDTO>
            {
                Items = users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surnames = u.Surnames,
                    Email = u.Email,
                    Username = u.Username,
                    CreatedAt = u.CreatedAt,
                    Image = u.Image,
                    Role = u.Role.ToString(),
                    Enabled = u.Enabled
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<UserDTO> ToggleEnabledAsync(Guid requesterId, Guid targetUserId)
        {
            
            var requester = await _userRepository.GetByIdAsync(requesterId);

            if (requester == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            if (requester.Role != Role.Admin)
                throw new UnauthorizedAccessException("Acceso denegado. Se requiere rol Admin.");

            
            var updatedUser = await _userRepository.ToggleEnabledAsync(targetUserId);

            if (updatedUser == null)
                throw new Exception("El usuario no existe");

           
            return new UserDTO
            {
                Id = updatedUser.Id,
                Name = updatedUser.Name,
                Surnames = updatedUser.Surnames,
                Email = updatedUser.Email,
                Username = updatedUser.Username,
                CreatedAt = updatedUser.CreatedAt,
                Image = updatedUser.Image,
                Role = updatedUser.Role.ToString(),
                Enabled = updatedUser.Enabled
            };
        }

    }
}
