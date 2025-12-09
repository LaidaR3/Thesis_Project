using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace User_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst("id")?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new {
                message = "Your user profile",
                id = userId,
                email = email,
                role = role
            });
        }
    }
}
