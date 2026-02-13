using AuditService.Data;
using AuditService.Models;
using System.Security.Claims;

public class ForbiddenLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ForbiddenLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AuditDbContext db)
    {
        await _next(context);

        if (context.Response.StatusCode == 403)
        {
            var user = context.User;

            var log = new AuditLog
            {
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                ServiceName = "audit-service",
                Endpoint = context.Request.Path,
                HttpMethod = context.Request.Method,
                Result = "Access Denied - Insufficient Permissions",
                Timestamp = DateTime.UtcNow
            };

            db.AuditLogs.Add(log);
            await db.SaveChangesAsync();
        }
    }
}
