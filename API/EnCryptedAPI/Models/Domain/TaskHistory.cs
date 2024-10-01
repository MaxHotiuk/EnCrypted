using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("TaskHistory")]
public class TaskHistory
{
    [Key]
    public Guid HistoryID { get; set; }

    [Required]
    public Guid TaskID { get; set; }

    [ForeignKey("TaskID")]
    public virtual Task Task { get; set; } = null!;

    [Required]
    public Guid UserID { get; set; }

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}