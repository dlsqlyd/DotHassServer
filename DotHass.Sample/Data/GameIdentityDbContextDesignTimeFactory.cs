using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotHass.Sample.Data
{
    public class GameIdentityDbContextDesignTimeFactory : IDesignTimeDbContextFactory<GameIdentityDbContext>
    {
        public GameIdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<GameIdentityDbContext>();
            builder.UseMySql("Server=localhost;User Id=root;Password=123456;Database=dothass_identity");
            return new GameIdentityDbContext(builder.Options);
        }
    }
}