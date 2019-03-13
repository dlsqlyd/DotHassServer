using DotHass.Sample.Model.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotHass.Sample.Data
{
    public class GameIdentityDbContext : IdentityDbContext<GameUser>
    {
        public const string ConnectionStringName = "IdentityConnection";

        public GameIdentityDbContext(DbContextOptions<GameIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GameUser>(b =>
            {
                b.ToTable("gameuser");
            });
        }
    }
}