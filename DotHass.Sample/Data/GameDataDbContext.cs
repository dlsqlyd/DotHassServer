using DotHass.Repository.DB;
using DotHass.Sample.Model.Data;
using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;

namespace DotHass.Sample.Data
{
    public class GameDataDbContext : DataDbContext
    {
        public DbSet<GameRole> GameRole { get; set; }


        public GameDataDbContext(DbContextOptions<GameDataDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<GameRole>(build =>
            {
                build.HasKey(t => new { t.Id });
            });


            builder.AddJsonFields();
        }
    }
}