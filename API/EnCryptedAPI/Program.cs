using EnCryptedAPI.Data;
using EnCryptedAPI.Controllers;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EnCryptedDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("localDb")));

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

