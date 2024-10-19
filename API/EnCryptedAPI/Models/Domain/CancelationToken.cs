using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("CancelationTokens")]
public class CancelationToken
{
    [Key]
    public Guid TokenID { get; set; }

    [Required]
    public Guid TaskID { get; set; }

    [ForeignKey("TaskID")]
    public virtual Task Task { get; set; } = null!;

    [Required]
    public CancellationTokenSource CancellationTokenSource { get; set; } = null!;
}