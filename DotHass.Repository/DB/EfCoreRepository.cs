using DotHass.Repository.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Repository.DB
{
    public class EfCoreRepository<TDbContext, TEntity> : IRepository<TEntity>, IAsyncRepository<TEntity>, IFindExpressionRepository<TEntity> where TEntity : class where TDbContext : DbContext
    {
        private readonly DbContext _context;
        public virtual DbContext Context => _context;

        private readonly DbSet<TEntity> _set;

        protected virtual DbSet<TEntity> DbSet => _set;

        public EfCoreRepository(TDbContext dbContext)
        {
            _context = dbContext;
            _set = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entityToAdd, CancellationToken cancellationToken)
        {
            if (entityToAdd == null)
                throw new ArgumentNullException(nameof(entityToAdd));

            await Context.AddAsync(entityToAdd, cancellationToken).ConfigureAwait(false);

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entityToAdd;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken)
        {
            if (entitiesToAdd == null)
                throw new ArgumentNullException(nameof(entitiesToAdd));

            List<TEntity> entitiesToAddList = entitiesToAdd as List<TEntity> ?? entitiesToAdd.ToList();

            await Context.AddRangeAsync(entitiesToAddList, cancellationToken).ConfigureAwait(false);

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entitiesToAddList;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken)
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entityToUpdate;
        }

        public virtual async Task<TEntity> DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException(nameof(entityToDelete));

            Attach(entityToDelete);
            Context.Entry(entityToDelete).State = EntityState.Deleted;

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entityToDelete;
        }

        public virtual void Detach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Attach(entity);

            Context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Attach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (Context.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);
        }

        public virtual TEntity Add(TEntity entityToAdd)
        {
            if (entityToAdd == null)
                throw new ArgumentNullException(nameof(entityToAdd));

            Context.Add(entityToAdd);

            SaveChanges();

            return entityToAdd;
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entitiesToAdd)
        {
            if (entitiesToAdd == null)
                throw new ArgumentNullException(nameof(entitiesToAdd));

            List<TEntity> entitiesToAddList = entitiesToAdd as List<TEntity> ?? entitiesToAdd.ToList();

            Context.AddRange(entitiesToAddList);

            SaveChanges();

            return entitiesToAddList;
        }

        public virtual TEntity Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;

            SaveChanges();

            return entityToUpdate;
        }

        public virtual TEntity Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException(nameof(entityToDelete));

            Attach(entityToDelete);
            Context.Entry(entityToDelete).State = EntityState.Deleted;

            SaveChanges();
            return entityToDelete;
        }

        protected virtual void SaveChanges()
        {
            Context.ChangeTracker.DetectChanges();
            Context.SaveChanges();
        }

        /// <summary>
        /// Unit of work is being handled by implicit unit of work implementation. SaveChanges is a non public method which is not present in Repository contract.
        /// </summary>
        protected virtual Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            Context.ChangeTracker.DetectChanges();
            return Context.SaveChangesAsync(cancellationToken);
        }

        public virtual TEntity Find(params object[] ids)
        {
            return DbSet.Find(ids);
        }

        public virtual async Task<TEntity> FindAsync(params object[] ids)
        {
            return await DbSet.FindAsync(ids);
        }

        public virtual void Reload(TEntity entity)
        {
            Attach(entity);

            Context.Entry(entity).Reload();
        }

        public virtual async Task ReloadAsync(TEntity entity, CancellationToken cancellationToken)
        {
            Attach(entity);

            await Context.Entry(entity).ReloadAsync(cancellationToken);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return DbSet.Where(match).Single();
        }

        public IQueryable<TEntity> FindQuery()
        {
            return DbSet;
        }
    }
}