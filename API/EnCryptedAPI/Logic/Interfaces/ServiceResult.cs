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

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult<T> Success(T data, string message = "") =>
        new() { IsSuccess = true, Data = data, Message = message };

    public static ServiceResult<T> Failure(string message, List<string>? errors = null) =>
        new() { IsSuccess = false, Message = message, Errors = errors ?? [] };
}