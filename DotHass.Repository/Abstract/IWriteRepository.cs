using System.Collections.Generic;

namespace DotHass.Repository.Abstract
{
    public interface IWriteRepository<TEntity> where TEntity : class
    {
        //create
        TEntity Add(TEntity entityToAdd);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entitiesToAdd);

        //update

        TEntity Update(TEntity entityToUpdate);

        //delete
        TEntity Delete(TEntity entityToDelete);
    }
}