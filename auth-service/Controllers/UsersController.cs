using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;

namespace auth_service.Controllers
  
{
    [ApiController]
    [Route("api/auth/users")]
    public class UsersController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public UsersController(AuthDbContext context)
        {
            _context = context;
        }

        // 🔐 ADMIN ONLY: Get all users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // 
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var callerId = User.FindFirst("id")?.Value;
            var callerRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            // Only admin OR the same logged-in user can access this
            if (callerRole != "Admin" && callerId != id.ToString())
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }

        // update user
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updated)
        {
            var callerId = User.FindFirst("id")?.Value;
            var callerRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (callerRole != "Admin" && callerId != id.ToString())
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = updated.FirstName;
            user.LastName = updated.LastName;
            user.Email = updated.Email;

            // Only admin can update user roles
            if (callerRole == "Admin")
                user.Role = updated.Role;

       
            await _context.SaveChangesAsync();

            return Ok(user);
        }

   
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
