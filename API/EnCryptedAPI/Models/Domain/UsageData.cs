using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("UsageData")]
public class UsageData
{
    [Key]
    public Guid DataID { get; set; }

    [Required, MaxLength(100)]
    public string ActionType { get; set; } = string.Empty;

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(50)]
    public string Role { get; set; } = string.Empty;
}