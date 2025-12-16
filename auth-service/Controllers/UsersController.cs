using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using auth_service.Models;
using System.Security.Claims;

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

       
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

      
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (callerRole != "Admin" && callerId != id.ToString())
                return Forbid();

            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name)
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

      
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, new
            {
                newUser.Id,
                newUser.Email
            });
        }

       
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updated)
        {
            var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (callerRole != "Admin" && callerId != id.ToString())
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = updated.FirstName;
            user.LastName = updated.LastName;
            user.Email = updated.Email;

       

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully" });
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
