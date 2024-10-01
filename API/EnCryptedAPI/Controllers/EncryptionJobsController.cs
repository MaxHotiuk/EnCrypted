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

namespace EnCryptedAPI.Controllers
{
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

        [HttpPost("encrypt")]
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
                DataEncrypted = false,
                EncryptedData = request.Data,
                CreatedAt = DateTime.UtcNow
            };
            await _context.EncryptionJobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return Ok(job);
        }

        [HttpPost("decrypt")]
        [Authorize]
        public async Task<IActionResult> GetDecryptedData([FromBody] EncryptDataDto request)
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
                DataEncrypted = true,
                DecryptedData = request.Data,
                CreatedAt = DateTime.UtcNow
            };
            await _context.EncryptionJobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return Ok(job);
        }

        [HttpPut("do")]
        [Authorize]
        public async Task<IActionResult> DoTask([FromBody] EncryptRequestDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var job = await _context.EncryptionJobs.FirstOrDefaultAsync(j => j.JobID == request.encryptionJobId);
            if (job == null)
            {
                return NotFound();
            }

            if (!job.DataEncrypted)
            {
                job.EncryptedData = job.DecryptedData != null ? Encryption.Encryption.Encrypt(job.DecryptedData, request.passPhrase) : null;
            }
            else
            {
                job.DecryptedData = job.EncryptedData != null ? Encryption.Encryption.Decrypt(job.EncryptedData, request.passPhrase) : null;
            }

            await _context.SaveChangesAsync();

            return Ok(job);
        }

        //get all enc jobs for task
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
    }
}