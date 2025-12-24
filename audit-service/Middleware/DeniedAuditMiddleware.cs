using System.Security.Claims;
using AuditService.Data;
using AuditService.Models;

public class DeniedAuditMiddleware
{
    private readonly RequestDelegate _next;

    public DeniedAuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuditDbContext db)
    {
        await _next(context);

        if (context.Response.StatusCode != StatusCodes.Status401Unauthorized &&
            context.Response.StatusCode != StatusCodes.Status403Forbidden)
        {
            return;
        }

        // Zero Trust: log denied access when credentials were presented
        if (!context.Request.Headers.ContainsKey("Authorization"))
            return;

        var auditLog = new AuditLog
        {
            UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = context.User.FindFirst(ClaimTypes.Email)?.Value,
            Role = context.User.FindFirst(ClaimTypes.Role)?.Value,

            ServiceName = context.User.Identity?.Name ?? "UnknownService",
            Endpoint = context.Request.Path,
            HttpMethod = context.Request.Method,

            Result = "DENIED",
            Timestamp = DateTime.UtcNow
        };

        db.AuditLogs.Add(auditLog);
        await db.SaveChangesAsync();
    }
}
