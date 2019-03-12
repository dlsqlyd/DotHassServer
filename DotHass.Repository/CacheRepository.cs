using DotHass.Repository.Abstract;
using DotHass.Repository.Collection;
using DotHass.Repository.Entity;
using System;
using System.Collections.Generic;

namespace DotHass.Repository
{
    public class CacheRepository<TEntity> : ReadonlyCacheRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
    {
        public CacheRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Container = new CacheContainer(false);
        }

        public TEntity Add(TEntity value)
        {
            if (!RunAspect(attribute => attribute.OnAddExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            if (!TryAddEntity(key, value))
            {
                return default;
            }

            RunAspect(attribute => attribute.OnAddExecuted(value));

            return value;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entitys)
        {
            if (!RunAspect(attribute => attribute.OnAddRangeExecuting(entitys)))
                return default;

            foreach (var t in entitys)
            {
                string key = EntityKeyInfo.GetPrimaryKey(t);
                TryAddEntity(key, t);
            }

            RunAspect(attribute => attribute.OnAddRangeExecuted(entitys));

            return entitys;
        }

        public TEntity Delete(params object[] keys)
        {
            var value = Find(keys);

            return Delete(value);
        }

        public TEntity Delete(Func<TEntity, bool> match)
        {
            var value = Find(match);
            return Delete(value);
        }

        public TEntity Delete(TEntity value)
        {
            if (!RunAspect(attribute => attribute.OnDeleteExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            TryRemove(key);

            RunAspect(attribute => attribute.OnDeleteExecuted(value));

            return value;
        }

        public TEntity Update(TEntity value)
        {
            if (!RunAspect(attribute => attribute.OnUpdateExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            AddOrUpdate(key, value);

            RunAspect(attribute => attribute.OnUpdateExecuted(value));

            return value;
        }

        protected override bool LoadDataFactory(bool isReload)
        {
            return true;
        }

        protected override bool LoadItemFactory(object[] keys, bool isReplace)
        {
            var entity = RunAspect(attribute => attribute.OnFindExecuting<TEntity>(keys));
            string key = EntityKeyInfo.GetPrimaryKey(entity);
            TryAddEntity(key, entity);

            RunAspect(attribute => attribute.OnFindExecuted(entity));

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="entityData"></param>
        /// <param name="periodTime"></param>
        /// <param name="itemSet"></param>
        /// <param name="isLoad"></param>
        /// <returns></returns>
        public bool AddOrUpdate(string entityKey, TEntity entityData)
        {
            Collection.AddOrUpdate(entityKey, entityData, (k, t) => entityData);
            return true;
        }

        /// <summary>
        /// Remove entity from the cache, if use loaded from Redis
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool TryRemove(string groupKey, Func<TEntity, bool> callback = null)
        {
            if (Collection.TryRemove(groupKey, out TEntity itemSet))
            {
                if (callback != null)
                {
                    if (callback(itemSet))
                        return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityKey"></param>
        /// <param name="entityData"></param>
        /// <returns></returns>
        public bool TryAddEntity(string entityKey, TEntity entityData)
        {
            if (Collection.TryAdd(entityKey, entityData))
            {
                return true;
            }
            return false;
        }
    }
}