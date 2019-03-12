namespace DotHass.Repository.Abstract
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
    {
    }
}