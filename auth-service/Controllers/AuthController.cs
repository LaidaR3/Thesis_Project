using Microsoft.AspNetCore.Mvc;
using AuthService.Data;
using AuthService.Models;
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);
            if (user == null)
                return BadRequest("Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return BadRequest("Invalid email or password.");

            // FIXED: Pass the full User object
            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                token = token,
                email = user.Email,
                role = user.Role,
                id = user.Id
            });
        }

        [HttpPost("create-admin")]
public IActionResult CreateAdmin(RegisterDto dto)
{
    // Check if admin already exists
    var exists = _context.Users.FirstOrDefault(x => x.Email == dto.Email);
    if (exists != null)
        return BadRequest("User with this email already exists.");

    var admin = new User
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        Role = "Admin"
    };

    _context.Users.Add(admin);
    _context.SaveChanges();

    return Ok(new { message = "Admin created successfully!" });
}

    }
}
