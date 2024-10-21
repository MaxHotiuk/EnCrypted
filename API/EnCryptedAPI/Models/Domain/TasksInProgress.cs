using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EnCryptedAPI.Models.Domain;

[Table("TasksInProgress")]
public class TasksInProgress
{
    [Key]
    public Guid Id { get; set; }

    public required string ServerName { get; set; }
}