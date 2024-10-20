using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnCryptedAPI.Models.Domain
{
    public class CancellationToken
    {
        [Key]
        public Guid CancellationTokenID { get; set; }

        [Required]
        public Guid TaskID { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task Task { get; set; } = null!;

        [Required]
        public bool IsCanceled { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}