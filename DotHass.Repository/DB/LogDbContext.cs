using Microsoft.EntityFrameworkCore;

namespace DotHass.Repository.DB
{
    public class LogDbContext : DbContext
    {
        public const string ConnectionStringName = "LogConnection";

        public LogDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}