using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

using AuthService.Data;
using AuthService.DTOs;
using auth_service.Models;

using AuthService.Services;

using AuthService.DTOs.Audit;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly JwtService _jwt;
        private readonly AuditClient _audit;

        public AuthController(
            AuthDbContext context,
            JwtService jwt,
            AuditClient audit)
        {
            _context = context;
            _jwt = jwt;
            _audit = audit;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
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
            await _context.SaveChangesAsync();
        
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = 2 // User
            };
        
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        
            AttachServiceToken();
            await _audit.LogAsync(new CreateAuditLogDto
            {
                Action = "User Registered",
                TargetEndpoint = "/auth/register"
            });
        
            return Ok(new { message = "User registered successfully" });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefault(x => x.Email == dto.Email);
        
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    AttachServiceToken();
        
                    await _audit.LogAsync(new CreateAuditLogDto
                    {
                        Action = "Login Failed - Invalid Credentials",
                        TargetEndpoint = "/auth/login"
                    });
        
                    return Unauthorized("Invalid email or password.");
                }
        
                var token = _jwt.GenerateToken(user);
        
                AttachServiceToken();
                await _audit.LogAsync(new CreateAuditLogDto
                {
                    Action = "Login Successfully",
                    TargetEndpoint = "/auth/login"
                });
        
                return Ok(new
                {
                    token,
                    email = user.Email,
                    roles = user.UserRoles.Select(ur => ur.Role.Name),
                    id = user.Id
                });
            }
            catch
            {
                AttachServiceToken();
        
                await _audit.LogAsync(new CreateAuditLogDto
                {
                    Action = "LOGIN_FAILED_SYSTEM_ERROR",
                    TargetEndpoint = "/auth/login"
                });
        
                return StatusCode(503, "Authentication service unavailable.");
            }
        }
        



        [Authorize(Roles = "Admin,Service")]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto dto)
        {
            var exists = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
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
            await _context.SaveChangesAsync();

            var adminRole = new UserRole
            {
                UserId = admin.Id,
                RoleId = 1 
            };

            _context.UserRoles.Add(adminRole);
            await _context.SaveChangesAsync();

            // Audit who created the admin
            AttachServiceToken();
            await _audit.LogAsync(new CreateAuditLogDto
            {
                Action = "Admin Created",
                TargetEndpoint = "/auth/create-admin",
                Metadata = $"CreatedBy={User.FindFirst(ClaimTypes.Email)?.Value}"
            });

            return Ok(new { message = "Admin created successfully" });
        }



        [HttpPost("service-token")]
        public IActionResult GenerateServiceToken([FromQuery] string serviceName)
        {
            var token = _jwt.GenerateServiceToken(serviceName);
            return Ok(new { token });
        }

        private void AttachServiceToken()
        {
            var serviceToken = _jwt.GenerateServiceToken("auth-service");

            _audit.SetAuthorization(serviceToken);
        }
    }
}
