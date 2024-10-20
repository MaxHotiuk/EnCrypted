using EnCryptedAPI.Data;
using EnCryptedAPI.Requests;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnCryptedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly EnCryptedDbContext _context;

        public TaskController(EnCryptedDbContext context)
        {
            _context = context;
        }

        // POST: api/Task/create
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto taskDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authorized.");
            }

            var task = new Models.Domain.Task
            {
                TaskID = Guid.NewGuid(),
                TaskName = taskDto.Title,
                Description = taskDto.Description,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                UserID = Guid.Parse(userId),
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskID }, task);
        }

        // GET: api/Task/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TaskID == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (task.UserID.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized("You are not authorized to view this task.");
            }

            return Ok(new
            {
                task.TaskID,
                task.TaskName,
                task.Description,
                task.CreatedAt,
                task.IsCompleted,
                User = new { task.User.Id, task.User.UserName }
            });
        }

        // GET: api/Task/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTasksByUserId(Guid userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserID == userId)
                .ToListAsync();

            if (!tasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            var taskDtos = tasks.Select(task => new
            {
                task.TaskID,
                task.TaskName,
                task.Description,
                task.CreatedAt,
                task.IsCompleted,
            });

            return Ok(taskDtos);
        }

        //return all tasks for current user
        // GET: api/Task
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authorized.");
            }

            var tasks = await _context.Tasks
                .Where(t => t.UserID.ToString() == userId)
                .ToListAsync();

            if (!tasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            var taskDtos = tasks.Select(task => new
            {
                task.TaskID,
                task.TaskName,
                task.Description,
                task.CreatedAt,
                task.IsCompleted,
                task.Progress,
            });

            return Ok(taskDtos);
        }

        // PUT: api/Task/update/{id}
        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] TaskUpdateDto updateDto)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (task.UserID.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized("You are not authorized to update this task.");
            }

            task.IsCompleted = updateDto.IsCompleted;
            await _context.SaveChangesAsync();

            return Ok(new { task.TaskID, task.TaskName, task.IsCompleted });
        }

        // DELETE: api/Task/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (task.UserID.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized("You are not authorized to delete this task.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Task/delete
        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteAllTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authorized.");
            }

            var tasks = await _context.Tasks
                .Where(t => t.UserID.ToString() == userId)
                .ToListAsync();

            if (!tasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            _context.Tasks.RemoveRange(tasks);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
