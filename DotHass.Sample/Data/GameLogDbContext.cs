using DotHass.Repository.DB;
using Microsoft.EntityFrameworkCore;

namespace DotHass.Sample.Data
{
    public class GameLogDbContext : LogDbContext
    {
        public GameLogDbContext(DbContextOptions<GameLogDbContext> options) : base(options)
        {
        }
    }
}