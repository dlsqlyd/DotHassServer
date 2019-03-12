using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Repository.Abstract
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindAsync(params object[] keys);

        Task<TEntity> AddAsync(TEntity entityToAdd, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken);

        Task<TEntity> UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken);

        Task<TEntity> DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken);
    }
}