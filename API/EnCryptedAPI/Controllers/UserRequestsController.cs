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
    public class UserRequestsController : ControllerBase
    {
        private readonly EnCryptedDBContext _context;

        public UserRequestsController(EnCryptedDBContext context)
        {
            this._context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserRequests>), 200)]
        public IActionResult GetAllUserRequests()
        {
            var userRequests = _context.UserRequests.ToList();
            return Ok(userRequests);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserRequests), 200)]
        public IActionResult AddUserRequest(AddUserRequestsRequestDto request)
        {
            var userRequest = new UserRequests
            {
                Id = _context.UserRequests.Count() + 1,
                Text = request.Text,
                IsEncrypted = request.IsEncrypted,
                UserId = request.UserId,
            };

            _context.UserRequests.Add(userRequest);
            _context.SaveChanges();

            return Ok(userRequest);
        }

        [ProducesResponseType(200)]
        [HttpDelete]
        public IActionResult DeleteUserRequest(int id)
        {
            var userRequest = _context.UserRequests.FirstOrDefault(u => u.Id == id);
            if (userRequest == null)
            {
                return NotFound();
            }

            _context.UserRequests.Remove(userRequest);
            _context.SaveChanges();

            return Ok();
        }
    }
}