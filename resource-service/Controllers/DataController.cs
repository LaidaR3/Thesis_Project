using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace resource_service.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok(new { message = "Hello from Resource Service!" });
        }

      
        [Authorize]
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok(new { message = "Top secret data from Resource Service!" });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin-secret")]
        public IActionResult GetAdminSecret()
        {


            return Ok(new { message = "Admin-only secret data from Resource Service!" });
        }
    }
}
