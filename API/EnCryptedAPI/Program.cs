using EnCryptedAPI.Data;
using EnCryptedAPI.Controllers;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EnCryptedDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("localDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/users", (EnCryptedDBContext dbContext) =>
{
    var userController = new UsersController(dbContext);
    var users = userController.GetAllUsers();
    return Results.Ok(users);
})
.WithName("GetUsers")
.WithOpenApi();

app.MapPost("/api/users", (EnCryptedDBContext dbContext, AddUserRequestDto request) =>
{
    var userController = new UsersController(dbContext);
    var user = userController.AddUser(request);
    return Results.Ok(user);
})
.WithName("AddUser")
.WithOpenApi();

await app.RunAsync();

