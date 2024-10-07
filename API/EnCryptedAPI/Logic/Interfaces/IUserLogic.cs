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

public interface IUserLogic
{
    Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto registrationDto);
    Task<ServiceResult<UserLoginDto>> LoginUser(UserLoginDto loginDto);
    Task<ServiceResult<UserDetailDto>> GetUserDetail(Guid userId);
    Task<ServiceResult<IEnumerable<UserDetailDto>>> GetAllUsers();
    Task<ServiceResult<bool>> DeleteUser(Guid userId);
    Task<ServiceResult<bool>> IsUserInRole(Guid userId, string role);
}