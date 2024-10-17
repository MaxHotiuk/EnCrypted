using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("Tasks")]
public class Task
{
    [Key]
    public Guid TaskID { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [Required, MaxLength(100)]
    public string TaskName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }

    [Required]
    public bool IsCompleted { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public int Progress { get; set; } = 0;

    public virtual ICollection<EncryptionJob> EncryptionJobs { get; set; } = [];
    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = [];
}