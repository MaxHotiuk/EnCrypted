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
using System.Collections.Concurrent;

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogicController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly EnCryptedDbContext _context;
    private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> _taskCancellationTokens = new();

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
            return Unauthorized(new { message = "User not found or unauthorized" });
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

        return Ok(new { message = "Task created successfully", taskID = request.TaskID });
    }


    [HttpGet("task/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetEncryptionJobsForTask(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        var jobs = await _context.EncryptionJobs
            .Where(j => j.TaskID == taskId && j.UserID == user.Id)
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("task/{taskId}/progress")]
    [Authorize]
    public async Task<IActionResult> GetTaskProgress(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        var task = await _context.Tasks
            .Include(t => t.EncryptionJobs)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);

        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        if (task.UserID != user.Id)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        return Ok(task.Progress);
    }

    [HttpGet("dotask/{taskId}")]
    [Authorize]
    public async Task<IActionResult> EncryptOrDecryptData(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        var task = await _context.Tasks
            .Include(t => t.EncryptionJobs)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);

        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        if (task.UserID != user.Id)
        {
            return Unauthorized(new { message = "User not authorized for this task" });
        }

        if (task.IsCompleted)
        {
            return Ok(new { message = "Task already completed", taskID = taskId });
        }

        if (_taskCancellationTokens.TryGetValue(taskId, out _))
        {
            return BadRequest(new { message = "Task is already running" });
        }

        var cts = new CancellationTokenSource();
        _taskCancellationTokens[taskId] = cts;

        var request = new EncryptDataDto
        {
            TaskID = taskId,
            DataEncrypted = task.EncryptionJobs.Any(j => j.DataEncrypted),
            PassPhrase = task.EncryptionJobs.FirstOrDefault()?.PassPhrase ?? string.Empty
        };

        try
        {
            await EncryptionLogic.EncryptOrDecryptData(request, _context, cts.Token);
            task.IsCompleted = true;
            await _context.SaveChangesAsync();
        }
        catch (OperationCanceledException)
        {
            return BadRequest(new { message = "Task was canceled" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing the task", error = ex.Message });
        }
        finally
        {
            _taskCancellationTokens.TryRemove(taskId, out _);
        }

        return Ok(new { message = "Task completed successfully", taskID = request.TaskID });
    }

    [HttpPost("cancel/{taskId}")]
    [Authorize]
    public async Task<IActionResult> CancelTask(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskID == taskId);
        if (task == null || task.UserID != user.Id)
        {
            return NotFound(new { message = "Task not found or not authorized" });
        }

        if (task.IsCompleted)
        {
            return BadRequest(new { message = "Task is already completed" });
        }

        if (_taskCancellationTokens.TryRemove(taskId, out var tokenSource))
        {
            tokenSource.Cancel();
            task.Progress = -1;
            task.IsCompleted = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task cancelled successfully" });
        }

        return BadRequest(new { message = "Task is not currently running" });
    }

    [HttpGet("task/{taskId}/status")]
    [Authorize]
    public async Task<IActionResult> GetTaskStatus(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized" });
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskID == taskId);
        if (task == null || task.UserID != user.Id)
        {
            return NotFound(new { message = "Task not found or not authorized" });
        }

        var status = task.IsCompleted ? "Completed" :
                     _taskCancellationTokens.ContainsKey(taskId) ? "Running" : "Pending";

        return Ok(new { taskId = task.TaskID, status, progress = task.Progress });
    }

}