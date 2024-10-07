using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnCryptedAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EnCryptedAPI.Data;
public class EnCryptedDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public EnCryptedDbContext(DbContextOptions<EnCryptedDbContext> options)
        : base(options)
    {
    }

    public DbSet<EncryptionJob> EncryptionJobs { get; set; }
    public DbSet<Models.Domain.Task> Tasks { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }
    public DbSet<UsageData> UsageData { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<EncryptionJob>().ToTable("EncryptionJobs");
        builder.Entity<Models.Domain.Task>().ToTable("Tasks");
        builder.Entity<TaskHistory>().ToTable("TaskHistory");
        builder.Entity<UsageData>().ToTable("UsageData");
        builder.Entity<UserStatistic>().ToTable("Statistics");
        builder.Entity<UserSetting>().ToTable("UserSettings");
    }
}