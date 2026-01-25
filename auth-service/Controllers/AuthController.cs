using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

using AuthService.Data;
using AuthService.DTOs;
using auth_service.Models;

using AuthService.Services;

using AuthService.DTOs.Audit;
using System.Net.Http.Headers;


using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace AuthService.Controllers
{
    [ApiController]
[Route("api/auth")]
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
    var exists = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == dto.Email);

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

    var role = await _context.Roles
        .FirstOrDefaultAsync(r => r.Name == "User");

    if (role == null)
        return StatusCode(500, "Default role 'User' not found.");

    _context.UserRoles.Add(new UserRole
    {
        UserId = user.Id,
        RoleId = role.Id
    });

    await _context.SaveChangesAsync();

    // SAFE audit
    TryAudit("User Registered", "/auth/register");

    return Ok(new { message = "User registered successfully" });
}

        
        [HttpPost("login")]
public IActionResult Login(LoginDto dto)
{
    var user = _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(x => x.Email == dto.Email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
    {
        TryAudit("Login Failed - Invalid Credentials", "/auth/login");
        return Unauthorized("Invalid email or password.");
    }

    var token = _jwt.GenerateToken(user);

    TryAudit(
        "Login Successfully",
        "/auth/login",
        $"UserId={user.Id}, Roles={string.Join(",", user.UserRoles.Select(r => r.Role.Name))}"
    );

    return Ok(new
    {
        token,
        email = user.Email,
        roles = user.UserRoles.Select(ur => ur.Role.Name),
        id = user.Id
    });
}

private async Task TryAudit(string action, string endpoint, string? metadata = null)
{
    try
    {
        await _audit.LogAsync(new CreateAuditLogDto
        {
            Action = action,
            TargetEndpoint = endpoint,
            Metadata = metadata
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AUDIT FAILED] {ex.Message}");
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

    var adminRoleId = await _context.Roles
        .Where(r => r.Name == "Admin")
        .Select(r => r.Id)
        .FirstOrDefaultAsync();

    if (adminRoleId == 0)
        return StatusCode(500, "Admin role not configured.");

    _context.UserRoles.Add(new UserRole
    {
        UserId = admin.Id,
        RoleId = adminRoleId
    });

    await _context.SaveChangesAsync();

    await _audit.LogAsync(new CreateAuditLogDto
    {
        Action = "Admin Created",
        TargetEndpoint = "/api/auth/create-admin",
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

       
    }
}
