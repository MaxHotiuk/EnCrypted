using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class UserDetailDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required List<string> Roles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}