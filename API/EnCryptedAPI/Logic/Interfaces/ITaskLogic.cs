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

namespace EnCryptedAPI.Logic.Interfaces;

public interface ITaskLogic
{
    Task<ServiceResult<TaskCreateDto>> CreateTask(Guid userId, TaskCreateDto taskDto);
    Task<ServiceResult<TaskDto>> GetTaskById(Guid taskId, Guid userId);
    Task<ServiceResult<IEnumerable<TaskDto>>> GetUserTasks(Guid userId);
    Task<ServiceResult<TaskDto>> UpdateTaskStatus(Guid taskId, Guid userId, TaskUpdateDto updateDto);
    Task<ServiceResult<bool>> DeleteTask(Guid taskId, Guid userId);
}