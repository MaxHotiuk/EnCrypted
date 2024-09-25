using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnCryptedAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace EnCryptedAPI.Data
{
    public class EnCryptedDBContext : DbContext
    {
        public EnCryptedDBContext(DbContextOptions<EnCryptedDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }   
        public DbSet<UserRequests> UserRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>( entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
            modelBuilder.Entity<UserRequests>( entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.IsEncrypted).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
            });
        }
    }
}