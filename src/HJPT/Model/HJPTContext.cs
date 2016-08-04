using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Model  
{
    public class HJPTDbContext: DbContext
    {
        public HJPTDbContext(DbContextOptions<HJPTDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasAlternateKey(a => a.Username);
            modelBuilder.Entity<ApplicationUser>().HasAlternateKey(a => a.Email);
            modelBuilder.Entity<ApplicationUser>().HasKey(c => c.UID);
        }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
