using EnCryptedAPI.Data;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EnCryptedAPI.Requests;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserStatisticController : ControllerBase
{
    private readonly EnCryptedDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public UserStatisticController(EnCryptedDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateUserStatistic([FromBody] UserStatisticCreateDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userStatistic = new UserStatistic
        {
            UserId = user.Id,
            TotalTasksCompleted = request.TotalTasksCompleted,
            TotalTimeTracked = request.TotalTimeTracked,
            Date = DateTime.UtcNow
        };
        await _context.UserStatistics.AddAsync(userStatistic);
        await _context.SaveChangesAsync();

        return Ok(userStatistic);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserStatisticsForUser(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var userStatistics = await _context.UserStatistics.Where(x => x.UserId == userId).ToListAsync();
        return Ok(userStatistics);
    }

    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUserStatistics()
    {
        var userStatistics = await _context.UserStatistics.ToListAsync();
        return Ok(userStatistics);
    }

    [HttpPost("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUserStatistic([FromBody] UserStatisticUpdateDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userStatistic = await _context.UserStatistics.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Date.Date == DateTime.UtcNow.Date);
        if (userStatistic == null)
        {
            return NotFound();
        }

        userStatistic.TotalTasksCompleted = request.TotalTasksCompleted;
        userStatistic.TotalTimeTracked = request.TotalTimeTracked;
        await _context.SaveChangesAsync();

        return Ok(userStatistic);
    }
}