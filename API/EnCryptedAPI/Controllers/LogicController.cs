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
using EnCryptedAPI.Encryption;
using EnCryptedAPI.Logic;

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogicController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly EnCryptedDbContext _context;

    public LogicController(UserManager<User> userManager, EnCryptedDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("createtasklogic")]
    [Authorize]
    public async Task<IActionResult> CreateEncryptTaskData([FromBody] LogicTaskCreateDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var words = request.AllTextData.Split(' ');
        foreach (var word in words)
        {
            var job = new EncryptionJob
            {
                UserID = user.Id,
                TaskID = request.TaskID,
                DataEncrypted = request.IsEncrypted,
                EncryptedData = word,
                CreatedAt = DateTime.UtcNow,
                PassPhrase = request.PassPhrase
            };
            await _context.EncryptionJobs.AddAsync(job);
        }
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("task/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetEncryptionJobsForTask(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var jobs = await _context.EncryptionJobs
            .Where(j => j.TaskID == taskId && j.UserID == user.Id)
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpPut("dotask/{taskId}")]
    [Authorize]
    public async Task<IActionResult> EncryptOrDecryptData(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var task = await _context.Tasks
            .Include(t => t.EncryptionJobs)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);

        if (task == null)
        {
            return NotFound();
        }

        if (task.UserID != user.Id)
        {
            return Unauthorized();
        }

        var request = new EncryptDataDto
        {
            TaskID = taskId,
            DataEncrypted = task.EncryptionJobs.Any(j => j.DataEncrypted),
            PassPhrase = task.EncryptionJobs.FirstOrDefault()?.PassPhrase ?? string.Empty
        };

        await EncryptionLogic.EncryptOrDecryptData(request, _context);

        task.IsCompleted = true;

        return Ok();
    }

    [HttpGet("task/{taskId}/progress")]
    [Authorize]
    public async Task<IActionResult> GetTaskProgress(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var task = await _context.Tasks
            .Include(t => t.EncryptionJobs)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);

        if (task == null)
        {
            return NotFound();
        }

        if (task.UserID != user.Id)
        {
            return Unauthorized();
        }

        return Ok(task.Progress);
    }
}