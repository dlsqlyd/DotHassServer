namespace DotHass.Repository.DB
{
    public class LogDbRepository<T> : EfCoreRepository<LogDbContext, T> where T : class
    {
        public LogDbRepository(LogDbContext dbContext) : base(dbContext)
        {
        }
    }
}