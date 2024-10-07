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

public interface IUserStatisticLogic
{
    Task<ServiceResult<UserStatisticCreateDto>> CreateUserStatistic(Guid userId, UserStatisticCreateDto createDto);
    Task<ServiceResult<UserStatisticUpdateDto>> UpdateUserStatistic(Guid userId, UserStatisticUpdateDto updateDto);
    Task<ServiceResult<IEnumerable<UserStatisticDetailDto>>> GetUserStatistics(Guid userId);
    Task<ServiceResult<IEnumerable<UserStatisticDetailDto>>> GetAllUserStatistics();
}
