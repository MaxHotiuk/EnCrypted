using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("UserSettings")]
public class UserSetting
{
    [Key]
    public Guid SettingID { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [ForeignKey("UserID"), Required]
    public virtual User User { get; set; } = null!;

    [Required, MaxLength(20)]
    public string Theme { get; set; } = string.Empty;

    [Required]
    public bool NotificationsEnabled { get; set; }

    [Required, MaxLength(20)]
    public string PreferredLanguage { get; set; } = string.Empty;
}