using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace resource_service.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok(new { message = "Public data" });
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok(new { message = "Authenticated data" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-secret")]
        public IActionResult GetAdminSecret()
        {
            return Ok(new { message = "Admin-only secret data" });
        }
    }
}
