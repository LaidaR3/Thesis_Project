using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Services;
using System.Security.Claims;

namespace resource_service.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly AuditClient _audit;

        public DataController(AuditClient audit)
        {
            _audit = audit;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-secret")]
        public async Task<IActionResult> GetAdminSecret()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var email  = User.FindFirstValue(ClaimTypes.Email)!;
            var role   = User.FindFirstValue(ClaimTypes.Role)!;

            await _audit.LogAsync(
                userId,
                email,
                role,
                "/api/data/admin-secret",
                "GET",
                "Allowed"
            );

            return Ok(new { message = "Admin-only secret data" });
        }
    }
}
