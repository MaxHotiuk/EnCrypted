using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class AddUserRequestDto
    {
        public required string Username { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}