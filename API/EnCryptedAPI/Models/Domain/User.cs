using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Models.Domain;

[Table("Users")]
public class User : IdentityUser<Guid>
{
    [Key]
    public override Guid Id { get; set; }

    [Required, MaxLength(100)]
    public override string? UserName { get; set; }

    [Required, MaxLength(150)]
    public override string? Email { get; set; }

    [Required]
    public override string? PasswordHash { get; set; }

    [Required, MaxLength(50)]
    public string Role { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Task> Tasks { get; set; }
    public virtual ICollection<EncryptionJob> EncryptionJobs { get; set; }
    public virtual ICollection<UserSetting> UserSettings { get; set; }
    public virtual ICollection<TaskHistory> TaskHistories { get; set; }

    public User()
    {
        Role = string.Empty;
        Tasks = new HashSet<Task>();
        EncryptionJobs = new HashSet<EncryptionJob>();
        UserSettings = new HashSet<UserSetting>();
        TaskHistories = new HashSet<TaskHistory>();
    }
}