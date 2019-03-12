using Microsoft.EntityFrameworkCore;

namespace DotHass.Repository.DB
{
    public class DataDbContext : DbContext
    {
        public const string ConnectionStringName = "DataConnection";

        public DataDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}