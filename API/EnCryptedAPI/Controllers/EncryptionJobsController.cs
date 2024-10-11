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

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EncryptionJobsController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly EnCryptedDbContext _context;

    public EncryptionJobsController(UserManager<User> userManager, EnCryptedDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateEncryptTaskData([FromBody] EncryptDataDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var job = new EncryptionJob
        {
            UserID = user.Id,
            TaskID = request.TaskID,
            DataEncrypted = request.DataEncrypted,
            EncryptedData = request.Data,
            CreatedAt = DateTime.UtcNow,
            PassPhrase = request.PassPhrase
        };
        await _context.EncryptionJobs.AddAsync(job);
        await _context.SaveChangesAsync();

        return Ok(job);
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

        var jobs = await _context.EncryptionJobs.Where(j => j.TaskID == taskId && j.UserID == user.Id).ToListAsync();
        return Ok(jobs);
    }

    [HttpGet("job/{jobId}")]
    [Authorize]
    public async Task<IActionResult> GetEncryptionJob(Guid jobId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var job = await _context.EncryptionJobs.FirstOrDefaultAsync(j => j.JobID == jobId && j.UserID == user.Id);
        if (job == null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    [HttpDelete("job/{jobId}")]
    [Authorize]
    public async Task<IActionResult> DeleteEncryptionJob(Guid jobId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var job = await _context.EncryptionJobs.FirstOrDefaultAsync(j => j.JobID == jobId && j.UserID == user.Id);
        if (job == null)
        {
            return NotFound();
        }

        _context.EncryptionJobs.Remove(job);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("jobs")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllEncryptionJobs()
    {
        var jobs = await _context.EncryptionJobs.ToListAsync();
        return Ok(jobs);
    }
}
