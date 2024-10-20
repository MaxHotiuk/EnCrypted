using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class UpdatePasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}