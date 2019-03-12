using DotHass.Repository.Abstract;
using DotHass.Repository.Collection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotHass.Repository
{
    public abstract class ReadonlyCacheRepository<TEntity> : BaseCacheRepository<TEntity>, IReadOnlyRepository<TEntity> where TEntity : class
    {
        public ReadonlyCacheRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Container = new CacheContainer(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public TEntity Find(params object[] keys)
        {
            if (TryGetCacheItem(keys, true, out TEntity data))
            {
                return data;
            }
            return default;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public TEntity Find(Func<TEntity, bool> match)
        {
            CheckLoad();
            return Collection.Select(v => (TEntity)v.Value).Where(match).Single();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public List<TEntity> FindAll()
        {
            return FindAll(m => true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<TEntity> FindAll(Func<TEntity, bool> match)
        {
            CheckLoad();
            List<TEntity> list = Collection.Select(v => (TEntity)v.Value)
                .Where(match)
                .ToList();
            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool Exist(Func<TEntity, bool> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            CheckLoad();

            return Collection.Select(i => (TEntity)i.Value).Any(match);
        }
    }
}