using Microsoft.AspNetCore.Mvc;
using AuthService.Data;
using auth_service.Models;

using AuthService.Services;
using AuthService.DTOs;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly JwtService _jwt;

        public AuthController(AuthDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var exists = _context.Users.FirstOrDefault(x => x.Email == dto.Email);
            if (exists != null)
                return BadRequest("User with this email already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = 2 // User
            };

            _context.UserRoles.Add(userRole);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }


        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(x => x.Email == dto.Email);
        
            if (user == null)
                return BadRequest("Invalid email or password.");
        
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return BadRequest("Invalid email or password.");
        
            var token = _jwt.GenerateToken(user);
        
            return Ok(new
            {
                token,
                email = user.Email,
                roles = user.UserRoles.Select(ur => ur.Role.Name),
                id = user.Id
            });
        }
        
        
        
        

        [HttpPost("create-admin")]
        public IActionResult CreateAdmin(RegisterDto dto)
        {
            var exists = _context.Users.FirstOrDefault(x => x.Email == dto.Email);
            if (exists != null)
                return BadRequest("User with this email already exists.");

            var admin = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(admin);
            _context.SaveChanges();

            var adminRole = new UserRole
            {
                UserId = admin.Id,
                RoleId = 1 // Admin
            };

            _context.UserRoles.Add(adminRole);
            _context.SaveChanges();

            return Ok(new { message = "Admin created successfully!" });
        }


    }
}
