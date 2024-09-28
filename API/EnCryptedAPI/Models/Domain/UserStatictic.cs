using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("Statistics")]
public class UserStatistic
{
    [Key]
    public Guid StatsId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public int TotalTasksCompleted { get; set; } = 0;

    public int TotalTimeTracked { get; set; } = 0;

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}