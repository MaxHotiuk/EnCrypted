using EnCryptedAPI.Data;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(EnCryptedDbContext context, UserManager<User> userManager, IConfiguration configuration) : ControllerBase
{
    private readonly EnCryptedDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        var existingUser = await _userManager.FindByNameAsync(registrationDto.Username);

        if (existingUser != null)
        {
            return BadRequest("User with this username already exists.");
        }

        var user = new User
        {
            UserName = registrationDto.Username,
            Email = registrationDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
            CreatedAt = DateTime.UtcNow,
            LastLogin = null,
            Tasks = [],
            EncryptionJobs = []
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (registrationDto.Roles is null)
        {
            await _userManager.AddToRoleAsync(user, "registered");
        }
        else
        {
            foreach (var role in registrationDto.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
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

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            Token = token,
            Massage = "Login successful.",
            IsSuccess = true
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(new { user.Id, user.UserName, user.Email, user.LastLogin });
    }

    [HttpGet("detail")]
    [Authorize]
    public async Task<IActionResult> GetUserDetail()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized("User ID not found.");
        }

        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(new UserDetailDto
        {
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = [.. await _userManager.GetRolesAsync(user)],
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        });
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        if (!users.Any())
        {
            return NotFound("No users found.");
        }

        var userDtos = users.Select(async user => new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.LastLogin,
            Roles = await _userManager.GetRolesAsync(user)
        });

        return Ok(userDtos);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII
            .GetBytes(_configuration.GetSection("JwtSettings").GetSection("SecurityKey").Value!);

        var roles = _userManager.GetRolesAsync(user).Result;

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Aud,
                _configuration.GetSection("JwtSettings").GetSection("ValidAudience").Value!),
            new Claim(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JwtSettings").GetSection("ValidIssuer").Value!)
        ];

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
