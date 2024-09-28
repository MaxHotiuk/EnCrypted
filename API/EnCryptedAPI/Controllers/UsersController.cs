using EnCryptedAPI.Data;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EnCryptedAPI.Requests;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace EnCryptedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly EnCryptedDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(EnCryptedDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                Role = "registered",
                CreatedAt = DateTime.UtcNow,
                LastLogin = null,
                Tasks = new List<Models.Domain.Task>(),
                EncryptionJobs = new List<EncryptionJob>(),
                UserSettings = new List<UserSetting>(),
                TaskHistories = new List<TaskHistory>()
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return user info or token
            return Ok(new { user.Id, user.UserName, user.Email, user.LastLogin });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { user.Id, user.UserName, user.Email, user.LastLogin });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            var userDtos = users.Select(user => new 
            {
                user.Id,
                user.UserName,
                user.Email,
                user.LastLogin
            });

            return Ok(userDtos);
        }
    }
}
