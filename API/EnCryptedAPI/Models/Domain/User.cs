using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Models.Domain
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Username { get; set; }
        public string? Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}