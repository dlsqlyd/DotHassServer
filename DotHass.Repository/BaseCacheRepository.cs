using DotHass.Repository.Aspects;
using DotHass.Repository.Collection;
using DotHass.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotHass.Repository
{
    public abstract class BaseCacheRepository<TEntity> where TEntity : class
    {
        private readonly string _typeName;

        protected string TypeName
        {
            get { return _typeName; }
        }

        protected CacheContainer Container { get; set; }

        protected IDataCollection Collection
        {
            get
            {
                if (Container != null)
                {
                    return Container.Collection;
                }
                throw new Exception(string.Format("CacheContainer \"{0}\" is not created.", TypeName));
            }
        }

        public BaseCacheRepository(IServiceProvider serviceProvider)
        {
            var entityType = typeof(TEntity);
            _typeName = entityType.Name;
            Aspects = entityType.GetTypeInfo().GetAllAttributes<RepositoryActionBaseAttribute>(inherit: true)
             .OrderBy(x => x.Order)
             .ToDictionary(a => a.GetType().FullName, a => a);

            RunAspect(aspect => aspect.OnInitialized(new RepositoryActionContext<TEntity>(serviceProvider)));
        }

        /// <summary>
        /// 尝试取缓存项，若不存在则自动加载
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="isAutoLoad">是否要自动加载</param>
        /// <param name="itemData"></param>
        /// <param name="loadStatus"></param>
        /// <returns></returns>
        protected bool TryGetCacheItem<TValue>(object[] keys, bool isAutoLoad, out TValue itemData)
        {
            CheckLoad();
            itemData = default;

            string groupKey = EntityKeyInfo.CreateKeyCode(keys);

            if (string.IsNullOrEmpty(groupKey))
            {
                return false;
            }
            //处理分组方法加载，在内存中存在且加载不出错时，才不需要重读
            if (Collection.TryGetValue(groupKey, out object itemSet))
            {
                itemData = (TValue)itemSet;
                return true;
            }
            if (isAutoLoad && LoadItemFactory(keys, false))
            {
                if (Collection.TryGetValue(groupKey, out itemSet))
                {
                    itemData = (TValue)itemSet;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        protected void CheckLoad()
        {
            //兼容调用AutoLoad方法时加载的部分数据
            if (Container != null && !Container.HasLoadSuccess)
            {
                Load();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public LoadingStatus Load()
        {
            return Load(false);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private LoadingStatus Load(bool isReload)
        {
            if (Container != null)
            {
                if (Container.LoadingStatus != LoadingStatus.None)
                {
                    //如果是新增加的不能清除
                    Container.Collection.Clear();
                    Container.ResetStatus();
                }
                Container.OnLoadFactory(LoadDataFactory, isReload);
                return Container.LoadingStatus;
            }
            return LoadingStatus.None;
        }

        #region aspects

        protected readonly Dictionary<string, RepositoryActionBaseAttribute> Aspects;

        protected bool RunAspect(Func<RepositoryActionBaseAttribute, bool> action)
        {
            return Aspects.Values
                .Where(a => a.Enabled)
                .OrderBy(a => a.Order)
                .All(action);
        }

        protected void RunAspect(Action<RepositoryActionBaseAttribute> action)
        {
            var aspects = Aspects.Values
                .Where(a => a.Enabled)
                .OrderBy(a => a.Order);

            foreach (var attribute in aspects)
            {
                action(attribute);
            }
        }

        protected TEntity RunAspect(Func<RepositoryActionBaseAttribute, TEntity> action)
        {
            var aspects = Aspects.Values
                .Where(a => a.Enabled)
                .OrderBy(a => a.Order);

            TEntity result = default;

            foreach (var attribute in aspects)
            {
                result = action(attribute);
                //找到任意一个则退出
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        protected void DisableAspect(Type aspectType)
        {
            ValidateArgument(aspectType);
            var aspect = Aspects[aspectType.FullName];
            aspect.Enabled = false;
        }

        protected void EnableAspect(Type aspectType)
        {
            ValidateArgument(aspectType);
            var aspect = Aspects[aspectType.FullName];
            aspect.Enabled = true;
        }

        private void ValidateArgument(Type aspectType)
        {
            var baseAttribute = typeof(RepositoryActionBaseAttribute);

            if (!baseAttribute.GetTypeInfo().IsAssignableFrom(aspectType.GetTypeInfo()))
                throw new ArgumentException(string.Format("Only aspects derived from a type {0} are valid arguments", baseAttribute.Name));

            if (!Aspects.ContainsKey(aspectType.FullName))
                throw new InvalidOperationException(string.Format("There is no aspect of a type {0}", aspectType.Name));
        }

        protected void Error(Exception ex)
        {
            RunAspect(aspect => aspect.OnError(ex));
        }

        #endregion aspects

        protected abstract bool LoadDataFactory(bool isReload);

        protected abstract bool LoadItemFactory(object[] keys, bool isReplace);
    }
}