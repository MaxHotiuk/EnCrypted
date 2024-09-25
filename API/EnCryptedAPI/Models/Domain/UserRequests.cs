using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Models.Domain
{
    [Table("UserRequests")]
    public class UserRequests
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Text { get; set; }
        [Required]
        public required bool IsEncrypted { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public required int UserId { get; set; }
    }
}