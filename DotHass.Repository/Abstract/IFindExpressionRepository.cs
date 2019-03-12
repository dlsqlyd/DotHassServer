using System;
using System.Linq;
using System.Linq.Expressions;

namespace DotHass.Repository.Abstract
{
    public interface IFindExpressionRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> FindQuery();

        TEntity Find(Expression<Func<TEntity, bool>> match);
    }
}