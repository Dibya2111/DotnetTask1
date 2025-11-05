using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public UserController(EmployeeDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.UserEmail == user.UserEmail))
                return Conflict("User with this email already exists.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _context.Users.FindAsync(email);
            if (user == null)
                return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }
    }
}
