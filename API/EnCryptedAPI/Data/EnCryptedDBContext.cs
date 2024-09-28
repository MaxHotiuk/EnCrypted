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
    public DbSet<GeneralStatistic> GeneralStatistics { get; set; }
    public DbSet<Models.Domain.Task> Tasks { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }
    public DbSet<UsageData> UsageData { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<EncryptionJob>().ToTable("EncryptionJobs");
        modelBuilder.Entity<GeneralStatistic>().ToTable("GeneralStatistics");
        modelBuilder.Entity<Models.Domain.Task>().ToTable("Tasks");
        modelBuilder.Entity<TaskHistory>().ToTable("TaskHistory");
        modelBuilder.Entity<UsageData>().ToTable("UsageData");
        modelBuilder.Entity<UserStatistic>().ToTable("Statistics");
        modelBuilder.Entity<UserSetting>().ToTable("UserSettings");
    }
}