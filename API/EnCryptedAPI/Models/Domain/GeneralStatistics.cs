using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("GeneralStatistics")]
public class GeneralStatistic
{
    [Key]
    public Guid StatID { get; set; }

    [Required, MaxLength(100)]
    public string ActionType { get; set; } = string.Empty;

    [Required]
    public int Count { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
