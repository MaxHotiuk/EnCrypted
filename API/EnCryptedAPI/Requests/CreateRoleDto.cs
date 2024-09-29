using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        public required string RoleName { get; set; }
    }
}