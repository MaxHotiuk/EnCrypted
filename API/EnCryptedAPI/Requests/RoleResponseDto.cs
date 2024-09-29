using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class RoleResponseDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Role name is required")]
        public required string Name { get; set; }
        public int TotalUsers { get; set; }
    }
}