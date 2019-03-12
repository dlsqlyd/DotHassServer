namespace DotHass.Repository.Abstract
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        TEntity Find(params object[] keys);
    }
}