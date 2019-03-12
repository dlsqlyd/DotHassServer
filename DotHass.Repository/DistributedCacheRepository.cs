using CacheManager.Core;
using DotHass.Repository.Abstract;
using DotHass.Repository.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DotHass.Repository
{
    public class DistributedCacheRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ICacheManager<TEntity> cacheManager;
        private readonly ILogger<DistributedCacheRepository<TEntity>> _logger;
        private string region;

        public string Region
        {
            get
            {
                if (string.IsNullOrEmpty(region))
                {
                    region = typeof(TEntity).Name;
                }
                return region;
            }
        }

        public DistributedCacheRepository(ICacheManager<TEntity> manager, ILogger<DistributedCacheRepository<TEntity>> logger)
        {
            this.cacheManager = manager;
            this._logger = logger;
        }

        public TEntity Find(params object[] keys)
        {
            string key = EntityKeyInfo.CreateKeyCode(keys);
            var entity = cacheManager.Get<TEntity>(key, Region);
            return entity;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        public TEntity Add(TEntity entity)
        {
            var key = EntityKeyInfo.GetPrimaryKey(entity);
            cacheManager.Add(key, entity, Region);
            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entitiesToAdd)
        {
            foreach (var entity in entitiesToAdd)
            {
                var key = EntityKeyInfo.GetPrimaryKey(entity);
                cacheManager.Add(key, entity, Region);
            }
            return entitiesToAdd;
        }

        public TEntity Update(TEntity entity)
        {
            var key = EntityKeyInfo.GetPrimaryKey(entity);
            return cacheManager.AddOrUpdate(key, Region, entity, (oldValue) =>
              {
                  return entity;
              });
        }

        public TEntity Delete(TEntity entity)
        {
            var key = EntityKeyInfo.GetPrimaryKey(entity);
            cacheManager.Remove(key, Region);
            return entity;
        }

        #region disposed

        private bool _disposed;

        ~DistributedCacheRepository()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name, "该对象已经销毁");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                _disposed = true;
            }
        }

        #endregion disposed
    }
}