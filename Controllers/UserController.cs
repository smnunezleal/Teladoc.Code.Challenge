using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teladoc.Code.Challenge.Context;
using Teladoc.Code.Challenge.Models;

namespace Teladoc.Code.Challenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private static List<User> _users = new List<User>();

        public UserController(UserContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            // check for existing user with same email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict("User with same email already exists");
            }

            // check age requirement
            int age = DateTime.Today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            if (age < 18)
            {
                return BadRequest("User must be 18 years or older");
            }

            // add user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, user);
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            // retrieve user by email
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // calculate age
            int age = DateTime.Today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            user.Age = age;

            return Ok(user);
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUser(string email, [FromBody] User user)
        {
            // retrieve user by email
            User existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // update user properties
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.DateOfBirth = user.DateOfBirth;

            // save changes to database
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            // retrieve user by email
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // remove user from database
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
