using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuditService.Data;
using AuditService.Models;
using AuditService.DTOs;

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

      
        [Authorize(Roles = "Service")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAuditLogDto dto)
        {
            var auditLog = new AuditLog
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
        
                ServiceName = User.FindFirst("service_name")?.Value ?? "UnknownService",
        
                Endpoint = dto.TargetEndpoint,
                HttpMethod = HttpContext.Request.Method,
        
                Result = dto.Action,
                Timestamp = DateTime.UtcNow
            };
        
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        
            return Ok();
        }



        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.AuditLogs
                .OrderByDescending(x => x.Timestamp));
        }
    }
}
