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
    }
}