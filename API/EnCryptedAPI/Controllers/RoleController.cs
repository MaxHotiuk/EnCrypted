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

namespace EnCryptedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<User> _userManager;

    public RolesController(EnCryptedDbContext context, RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet("{roleName}")]
    [Authorize]
    public async Task<IActionResult> GetRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost("create")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roleDto)
    {
        if (string.IsNullOrEmpty(roleDto.RoleName))
        {
            return BadRequest("Role name is required");
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleDto.RoleName);

        if (roleExists)
        {
            return BadRequest("Role already exists");
        }

        var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleDto.RoleName));

        if (roleResult.Succeeded)
        {
            return CreatedAtAction(nameof(GetRole), new { roleName = roleDto.RoleName }, roleDto);
        }

        return BadRequest(roleResult.Errors);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleManager.Roles.Select(r => new RoleResponseDto
        {
            Id = r.Id,
            Name = r.Name!,
            TotalUsers = _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
        }).ToListAsync();

        return Ok(roles);
    }

    [HttpDelete("{roleName}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return NotFound();
        }

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("assign")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignDto roleAssignDto)
    {
        var user = await _userManager.FindByNameAsync(roleAssignDto.Username);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var role = await _roleManager.FindByNameAsync(roleAssignDto.Role);
        if (role == null)
        {
            return NotFound("Role not found");
        }

        if (role.Name == null)
        {
            return BadRequest("Role name is null");
        }

        var result = await _userManager.AddToRoleAsync(user, role.Name);

        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}