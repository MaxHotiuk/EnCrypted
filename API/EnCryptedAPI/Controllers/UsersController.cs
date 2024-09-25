using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnCryptedAPI.Data;
using EnCryptedAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using EnCryptedAPI.Requests;

namespace EnCryptedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly EnCryptedDBContext _context;
        public UsersController(EnCryptedDBContext context)
        {
            this._context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        public IActionResult GetAllUsers()
        {
            var contacts = _context.Users.ToList();
            return Ok(contacts);
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), 200)]
        public IActionResult AddUser(AddUserRequestDto request)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }
    }
}