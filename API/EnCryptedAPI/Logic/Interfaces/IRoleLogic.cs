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

public interface IRoleLogic
{
    Task<ServiceResult<RoleResponseDto>> CreateRole(CreateRoleDto roleDto);
    Task<ServiceResult<RoleResponseDto>> GetRole(string roleName);
    Task<ServiceResult<IEnumerable<RoleResponseDto>>> GetAllRoles();
    Task<ServiceResult<bool>> DeleteRole(string roleName);
    Task<ServiceResult<bool>> AssignRole(RoleAssignDto roleAssignDto);
}