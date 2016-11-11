using HJPT.Models;
using Microsoft.EntityFrameworkCore;

namespace HJPT.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Invite> Invites { get; set; }
        public DbSet<Torrent> Torrents { get; set; }
        public DbSet<Peer> Peers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<User>( b =>
            {
            });

            builder.Entity<Invite>( b => 
            {
                b.HasKey(i => i.Id);
                b.HasAlternateKey(i => i.Hash);
                b.Property(i => i.Inviter).IsRequired();
            });
        }
    }
}
