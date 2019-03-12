using DotHass.Repository.Abstract;
using DotHass.Repository.Aspects;
using DotHass.Repository.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotHass.Repository
{
    public class RepositoryActionAttribute : RepositoryActionBaseAttribute
    {
        public bool LoadFromDb { get; set; } = true;
        public Type DbType { get; set; }
        public Type DistributedCacheType { get; set; }

        private IServiceProvider services;
        private ILogger<RepositoryActionAttribute> _logger;

        public RepositoryActionAttribute()
        {
        }

        public override void OnInitialized<T>(RepositoryActionContext<T> context)
        {
            this.services = context.Services;
            this._logger = this.services.GetService(typeof(ILogger<RepositoryActionAttribute>)) as ILogger<RepositoryActionAttribute>;
        }

        public override bool OnAddExecuting<T>(T entity)
        {
            var cmRepository = services.GetService(typeof(DistributedCacheRepository<T>)) as IRepository<T>;
            var efRepository = services.GetService(DbType) as IWriteRepository<T>;

            //异步的
            try
            {

                Task.Run(() =>
                {
                    efRepository.Add(entity);
                });

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{entity.GetType()} add to entity error;");
            }
            return true;
        }

        public override bool OnAddRangeExecuting<T>(IEnumerable<T> entitys)
        {
            var cmRepository = services.GetService(typeof(DistributedCacheRepository<T>)) as IRepository<T>;
            var efRepository = services.GetService(DbType) as IWriteRepository<T>;

            cmRepository.AddRange(entitys);

            //异步的
            try
            {
                Task.Run(() =>
                {
                    efRepository.AddRange(entitys);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{typeof(T)} add range to entity error;");
            }
            return true;
        }

        public override bool OnUpdateExecuting<T>(T entity)
        {
            var cmRepository = services.GetService(typeof(DistributedCacheRepository<T>)) as IRepository<T>;
            var efRepository = services.GetService(DbType) as IWriteRepository<T>;

            cmRepository.Update(entity);

            //异步的
            try
            {
                Task.Run(() =>
                {
                    //因为是异步，多次修改同一个entity的话,会出现下面的问题
                    //A second operation started on this context before a previous operation completed. 
                    //Any instance members are not guaranteed to be thread safe.
                    lock (entity)
                    {
                        efRepository.Update(entity);
                    }
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{entity.GetType()} update to entity error;");
            }

            return true;
        }

        public override bool OnDeleteExecuting<T>(T entity)
        {
            var cmRepository = services.GetService(typeof(DistributedCacheRepository<T>)) as IRepository<T>;
            var efRepository = services.GetService(DbType) as IWriteRepository<T>;

            cmRepository.Delete(entity);
            //异步的
            try
            {
                Task.Run(() =>
                {
                    efRepository.Delete(entity);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{entity.GetType()} remove to entity error;");
            }
            return true;
        }

        public override T OnFindExecuting<T>(params object[] keyValues)
        {
            var cmRepository = services.GetService(typeof(DistributedCacheRepository<T>)) as IRepository<T>;
            var efRepository = services.GetService(DbType) as IRepository<T>;

            var entity = cmRepository.Find(keyValues);

            if (entity == null && LoadFromDb == true)
            {
                entity = efRepository.Find(keyValues);
                if (entity != null)
                {
                    cmRepository.Add(entity);
                }
            }
            return entity;
        }

        /// <summary>
        /// cachemanager 不支持findall
        /// 使用redis。HKEYS 查询所有的key然后再查找key是否包含keyvalue可以实现（待做）
        /// https://github.com/MichaCo/CacheManager/issues/225
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public override IEnumerable<T> OnFindAllExecuting<T>(params object[] keyValues)
        {
            var efRepository = services.GetService(DbType) as IFindExpressionRepository<T>;

            var props = EntityKeyInfo.GetGroupProps<T>();

            var query = efRepository.FindQuery();

            var item = Expression.Parameter(typeof(T), "item");
            Expression expression = null;
            for (int i = 0; i < props.Count; i++)
            {
                var prop = Expression.Property(item, props[i].Name);
                var soap = Expression.Constant(keyValues[i]);

                var equal = Expression.Equal(prop, soap);

                if (expression == null)
                {
                    expression = equal;
                }
                else
                {
                    expression = Expression.And(
                        expression,
                        equal
                    );
                }
            }
            var lambda = Expression.Lambda<Func<T, bool>>(expression, item);

            return query.Where(lambda).ToList();
        }
    }
}