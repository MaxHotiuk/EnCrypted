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
public class TaskHistoryController : ControllerBase
{
    private readonly EnCryptedDbContext _context;
    private readonly UserManager<User> _userManager;

    public TaskHistoryController(EnCryptedDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateTaskHistory([FromBody] TaskHistoryCreateDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var taskHistory = new TaskHistory
        {
            UserID = user.Id,
            TaskID = request.TaskID,
            Status = request.TaskStatus,
            CreatedAt = DateTime.UtcNow
        };
        await _context.TaskHistories.AddAsync(taskHistory);
        await _context.SaveChangesAsync();

        return Ok(taskHistory);
    }

    [HttpGet("task/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetTaskHistoriesForTask(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var taskHistories = await _context.TaskHistories.Where(th => th.TaskID == taskId && th.UserID == user.Id).ToListAsync();
        return Ok(taskHistories);
    }

    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllTaskHistories()
    {
        var taskHistories = await _context.TaskHistories.ToListAsync();
        return Ok(taskHistories);
    }
}