using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotHass.Sample.Data
{
    public class GameLogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<GameLogDbContext>
    {
        public GameLogDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<GameLogDbContext>();
            builder.UseMySql("Server=localhost;User Id=root;Password=123456;Database=dothass_log");
            return new GameLogDbContext(builder.Options);
        }
    }
}