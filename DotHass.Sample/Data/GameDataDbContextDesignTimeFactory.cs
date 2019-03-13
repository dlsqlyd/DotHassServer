using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotHass.Sample.Data
{
    public class GameDataDbContextDesignTimeFactory : IDesignTimeDbContextFactory<GameDataDbContext>
    {
        public GameDataDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<GameDataDbContext>();
            builder.UseMySql("Server=localhost;User Id=root;Password=123456;Database=dothass_data");
            return new GameDataDbContext(builder.Options);
        }
    }
}