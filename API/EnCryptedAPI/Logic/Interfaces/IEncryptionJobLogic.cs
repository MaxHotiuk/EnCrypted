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

public interface IEncryptionJobLogic
{
    Task<ServiceResult<EncryptDataDto>> CreateEncryptionJob(Guid userId, EncryptDataDto encryptDto);
    Task<ServiceResult<IEnumerable<EncryptionJobDto>>> GetTaskEncryptionJobs(Guid taskId, Guid userId);
    Task<ServiceResult<EncryptionJobDto>> GetEncryptionJob(Guid jobId, Guid userId);
    Task<ServiceResult<bool>> DeleteEncryptionJob(Guid jobId, Guid userId);
    Task<ServiceResult<IEnumerable<EncryptionJobDto>>> GetAllEncryptionJobs();
}