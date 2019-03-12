namespace DotHass.Repository.DB
{
    public class DataDbRepository<T> : EfCoreRepository<DataDbContext, T> where T : class
    {
        public DataDbRepository(DataDbContext dbContext) : base(dbContext)
        {
        }
    }
}