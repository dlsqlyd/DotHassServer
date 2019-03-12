using DotHass.Repository.Abstract;
using DotHass.Repository.Collection;
using DotHass.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotHass.Repository
{
    public class GroupCacheRepository<TEntity> : BaseCacheRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public GroupCacheRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Container = new CacheContainer(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TEntity Add(TEntity value)
        {
            string personalId = EntityKeyInfo.GetGroupKey(value);

            if (string.IsNullOrEmpty(personalId))
            {
                throw new ArgumentNullException("t", "t.PersonalId is null");
            }

            if (!RunAspect(attribute => attribute.OnAddExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            if (!TryAddGroup(personalId, key, value))
            {
                return default;
            }

            RunAspect(attribute => attribute.OnAddExecuted(value));

            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entitys)
        {
            if (!RunAspect(attribute => attribute.OnAddRangeExecuting(entitys)))
                return default;

            foreach (var t in entitys)
            {
                string personalId = EntityKeyInfo.GetGroupKey(t);
                string key = EntityKeyInfo.GetPrimaryKey(t);
                TryAddGroup(personalId, key, t);
            }

            RunAspect(attribute => attribute.OnAddRangeExecuted(entitys));

            return entitys;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TEntity Update(TEntity value)
        {
            string personalId = EntityKeyInfo.GetGroupKey(value);

            if (string.IsNullOrEmpty(personalId))
            {
                throw new ArgumentNullException("t", "t.PersonalId is null");
            }
            if (!RunAspect(attribute => attribute.OnUpdateExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            AddOrUpdateGroup(personalId, key, value);

            RunAspect(attribute => attribute.OnUpdateExecuted(value));

            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public TEntity Delete(object personalId, params object[] keys)
        {
            var value = Find(personalId, keys);

            return Delete(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public TEntity Delete(object personalId, Func<TEntity, bool> match)
        {
            var value = Find(personalId, match);

            return Delete(value);
        }

        /// <summary>
        /// 删除数据并移出缓存
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TEntity Delete(TEntity value)
        {
            string personalId = EntityKeyInfo.GetGroupKey(value);

            if (string.IsNullOrEmpty(personalId))
            {
                throw new ArgumentNullException("t", "t.PersonalId is null");
            }
            if (!RunAspect(attribute => attribute.OnDeleteExecuting(value)))
                return default;

            string key = EntityKeyInfo.GetPrimaryKey(value);
            TryRemove(personalId, key);

            RunAspect(attribute => attribute.OnDeleteExecuted(value));

            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public TEntity Find(params object[] keys)
        {
            var personalId = keys[0];

            TEntity data = default;

            if (TryGetGroup(new object[] { personalId }, out IDataCollection collection))
            {
                collection.TryGetValue(EntityKeyInfo.CreateKeyCode(keys), out data);
            }
            return data;
        }

        /// <summary>
        /// 查找第一个匹配数据
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public TEntity Find(object personalId, Func<TEntity, bool> match)
        {
            TEntity t = default;

            if (TryGetGroup(new object[] { personalId }, out IDataCollection collection))
            {
                t = collection.Select(v => (TEntity)v.Value).Where(match).Single();
            }
            return t;
        }

        /// <summary>
        /// 查找所有匹配数据
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="isSort"></param>
        /// <returns></returns>
        public List<TEntity> FindAll(object personalId)
        {
            CheckLoad();
            if (TryGetGroup(new object[] { personalId }, out IDataCollection collection))
            {
                return collection.Select(k => (TEntity)k.Value).ToList();
            }
            return new List<TEntity>();
        }

        /// <summary>
        /// 查找所有匹配数据
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="match"></param>
        /// <param name="isSort"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<TEntity> FindAll(object personalId, Func<TEntity, bool> match)
        {
            CheckLoad();
            List<TEntity> list = new List<TEntity>();
            if (TryGetGroup(new object[] { personalId }, out IDataCollection collection))
            {
                list = collection.Select(v => (TEntity)v.Value).Where(match).ToList();
            }
            return list;
        }

        /// <summary>
        /// 在整个缓存中查找,不加载数据
        /// </summary>
        /// <param name="match">查找匹配条件</param>
        /// <returns></returns>
        public List<TEntity> FindAll(Func<TEntity, bool> match)
        {
            CheckLoad();
            List<TEntity> list = new List<TEntity>();
            foreach (var group in Collection)
            {
                list.AddRange(
                    ((IDataCollection)group.Value)
                    .Select(v => (TEntity)v.Value)
                    .Where(match)
                    .ToList());
            }
            return list;
        }

        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns></returns>
        public bool Exist(object personalId)
        {
            return TryGetGroup(new object[] { personalId }, out IDataCollection enityGroup);
        }

        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool Exist(string personalId, params object[] keys)
        {
            var entiry = Find(personalId, keys);
            return entiry != null;
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
            foreach (var group in Collection)
            {
                var value = ((IDataCollection)group.Value).Select(v => (TEntity)v.Value).Any(match);
                if (value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove entity from the cache, if use loaded from Redis
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool TryRemove(string groupKey, string key)
        {
            if (Collection.TryGetValue(groupKey, out IDataCollection items))
            {
                if (items.TryRemove(key, out TEntity entityData))
                {
                    if (items.Count == 0 && Collection.TryRemove(groupKey, out items))
                    {
                        items.Dispose();
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 分组集合模型
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="enityGroup"></param>
        /// <param name="loadStatus"></param>
        /// <returns></returns>
        public bool TryGetGroup(object[] keys, out IDataCollection enityGroup)
        {
            return TryGetCacheItem(keys, true, out enityGroup);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public IDataCollection InitGroupContainer(string groupKey)
        {
            var lazy = new Lazy<IDataCollection>(() =>
            {
                return new ConcurrentCacheCollection();
            });

            return Collection.GetOrAdd(groupKey, name => lazy.Value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="key"></param>
        /// <param name="entityData"></param>
        /// <returns></returns>
        public bool TryAddGroup(string groupKey, string key, TEntity entityData)
        {
            IDataCollection itemSet = InitGroupContainer(groupKey);
            if (itemSet != null)
            {
                if (!Equals(entityData, default(TEntity)) && itemSet.TryAdd(key, entityData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="key"></param>
        /// <param name="entityData"></param>
        public void AddOrUpdateGroup(string groupKey, string key, TEntity entityData)
        {
            IDataCollection itemSet = InitGroupContainer(groupKey);
            if (itemSet != null && !Equals(entityData, default(TEntity)))
            {
                itemSet.AddOrUpdate(key, entityData, (k, t) => entityData);
            }
        }

        protected override bool LoadDataFactory(bool isReload)
        {
            return true;
        }

        protected override bool LoadItemFactory(object[] keys, bool isReplace)
        {
            var aspects = Aspects.Values
               .Where(a => a.Enabled)
               .OrderBy(a => a.Order);

            IEnumerable<TEntity> result = new List<TEntity>();

            foreach (var attribute in aspects)
            {
                result = attribute.OnFindAllExecuting<TEntity>(keys);

                if (result != null)
                {
                    break;
                }
            }
            string groupKey = EntityKeyInfo.CreateKeyCode(keys);
            InitGroupContainer(groupKey);
            foreach (var entity in result)
            {
                string personalId = EntityKeyInfo.GetGroupKey(entity);
                string key = EntityKeyInfo.GetPrimaryKey(entity);
                TryAddGroup(personalId, key, entity);
            }

            RunAspect(attribute => attribute.OnFindAllExecuted(result));

            return true;
        }
    }
}