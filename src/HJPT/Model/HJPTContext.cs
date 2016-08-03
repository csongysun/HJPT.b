using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Model
{
    public class HJPTContext: DbContext
    {
        public HJPTContext(DbContextOptions<HJPTContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(a => a.Username);
            modelBuilder.Entity<User>().HasAlternateKey(a => a.Email);
            modelBuilder.Entity<User>().HasKey(c => c.UID);
        }
        public DbSet<User> Users { get; set; }
    }
}
