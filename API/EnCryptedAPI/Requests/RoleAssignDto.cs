using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class RoleAssignDto
    {
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}