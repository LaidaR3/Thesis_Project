using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuditService.Data;
using AuditService.Models;
using AuditService.DTOs;

using System.Security.Claims;



namespace AuditService.Controllers
{
[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly AuditDbContext _context;

    public AuditController(AuditDbContext context)
    {
        _context = context;
    }

    // Only SERVICES can write logs
    [Authorize(Roles = "Service")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAuditLogDto dto)
    {
        var auditLog = new AuditLog
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
    
            ServiceName = User.Identity?.Name ?? "UnknownService",
            Endpoint = HttpContext.Request.Path,
            HttpMethod = HttpContext.Request.Method,
    
            Result = dto.Result ?? "Success",
            Timestamp = DateTime.UtcNow
        };
    
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    
        return Ok();
    }

    // Only ADMINS can read logs
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.AuditLogs.OrderByDescending(x => x.Timestamp));
    }
}
}