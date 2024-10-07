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

public interface ITaskHistoryLogic
{
    Task<ServiceResult<TaskHistoryCreateDto>> CreateTaskHistory(Guid userId, TaskHistoryCreateDto createDto);
    Task<ServiceResult<IEnumerable<TaskHistoryDto>>> GetTaskHistories(Guid taskId, Guid userId);
    Task<ServiceResult<IEnumerable<TaskHistoryDto>>> GetAllTaskHistories();
}