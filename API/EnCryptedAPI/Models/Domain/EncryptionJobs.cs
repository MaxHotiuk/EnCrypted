using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("EncryptionJobs")]
public class EncryptionJob
{
    [Key]
    public Guid JobID { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [Required]
    public Guid TaskID { get; set; }

    [ForeignKey("TaskID")]
    public virtual Task Task { get; set; } = null!;

    [Required]
    public bool DataEncrypted { get; set; }

    public string? EncryptedData { get; set; }
    public string? DecryptedData { get; set; }

    [Required]
    public required string PassPhrase { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}