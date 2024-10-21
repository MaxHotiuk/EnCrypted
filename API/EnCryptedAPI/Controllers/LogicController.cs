using EnCryptedAPI.Data;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EnCryptedAPI.Requests;
using EnCryptedAPI.Encryption;
using EnCryptedAPI.Logic;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EnCryptedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogicController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly EnCryptedDbContext _context;
        public static int tasksInProcess = 0;

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
            tasksInProcess++;
            _context.TasksInProgress.Add(new TasksInProgress
            {
                ServerName = Environment.MachineName
            });

            var cancellationToken = new Models.Domain.CancellationToken
            {
                TaskID = taskId,
                IsCanceled = false,
                CreatedAt = DateTime.UtcNow
            };
            await _context.CancellationTokens.AddAsync(cancellationToken);
            await _context.SaveChangesAsync();

            var request = new EncryptDataDto
            {
                TaskID = taskId,
                DataEncrypted = task.EncryptionJobs.Any(j => j.DataEncrypted),
                PassPhrase = task.EncryptionJobs.FirstOrDefault()?.PassPhrase ?? string.Empty
            };

            try
            {
                await EncryptionLogic.EncryptOrDecryptData(request, _context);
            }
            catch (OperationCanceledException)
            {
                cancellationToken.IsCanceled = true;
                await _context.SaveChangesAsync();
                return BadRequest(new { message = "Task was canceled" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the task", error = ex.Message });
            }
            finally
            {
                tasksInProcess--;
                var taskInProgress = await _context.TasksInProgress
                    .FirstOrDefaultAsync(t => t.ServerName == Environment.MachineName);
                if (taskInProgress != null)
                {
                    _context.TasksInProgress.Remove(taskInProgress);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(new { message = "Task started successfully", taskID = request.TaskID });
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

            var cancellationToken = await _context.CancellationTokens
                .FirstOrDefaultAsync(ct => ct.TaskID == taskId);
            if (cancellationToken != null)
            {
                cancellationToken.IsCanceled = true;
                await _context.SaveChangesAsync();
                task.Progress = -1;
                task.IsCompleted = true;
                await _context.SaveChangesAsync();
                var existingCancellationToken = await _context.CancellationTokens
                    .FirstOrDefaultAsync(ct => ct.TaskID == taskId);
                if (existingCancellationToken != null && existingCancellationToken.IsCanceled)
                {
                    return Ok(new { message = "Task cancelled successfully" });
                }
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

            var cancellationToken = await _context.CancellationTokens
                .FirstOrDefaultAsync(ct => ct.TaskID == taskId);

            var status = task.IsCompleted ? "Completed" :
                         cancellationToken != null && !cancellationToken.IsCanceled ? "Running" : "Pending";

            return Ok(new { taskId = task.TaskID, status, progress = task.Progress });
        }
        //api/Logic/tasksinprocess
        [HttpGet("tasksinprocess")]
        public IActionResult GetTasksInProcess()
        {
            return Ok(new { tasksInProcess });
        }

        [HttpGet("tasksinprogressonallservers")]
        public async Task<IActionResult> GetTasksInProgressOnAllServers()
        {
            var tasksInProgress = await _context.TasksInProgress
                .GroupBy(task => task.ServerName)
                .Select(group => new 
                {
                    ServerName = group.Key,
                    TaskCount = group.Count()
                })
                .ToListAsync();
            if (tasksInProgress.Count == 0)
            {
                return NotFound(new { message = "No tasks in progress on any server" });
            }
            return Ok(new { tasksInProgress });
        }
    }
}
