using System;

namespace DotHass.Repository.Collection
{
    /// <summary>
    /// 缓存容器对象，每种T类型实体分配一个容器对象；
    ///
    /// 为什么在IDataCollection上面还要包一层？
    /// 每个container都有状态等等。
    ///
    /// IDataCollection只负责基本数据的操作
    /// </summary>
    [Serializable]
    public class CacheContainer : IDisposable
    {
        private IDataCollection _collection;

        internal CacheContainer(bool isReadOnly)
        {
            if (isReadOnly == true)
            {
                _collection = new ReadonlyCacheCollection();
            }
            else
            {
                _collection = new ConcurrentCacheCollection();
            }

            LoadingStatus = LoadingStatus.None;
        }

        /// <summary>
        /// 是否数据已加载成功
        /// </summary>
        public bool HasLoadSuccess
        {
            get { return LoadingStatus == LoadingStatus.Success; }
        }

        /// <summary>
        /// 加载状态
        /// </summary>
        public LoadingStatus LoadingStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _collection == null;
            }
        }

        /// <summary>
        /// 缓存项(CacheItemSet)的集合
        /// 主键:实体Key或PersonalId
        /// 键值：缓存项CacheItemSet（包括过期配置对象，缓存具体实体或实体集合）
        /// </summary>
        public IDataCollection Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return _collection.IsReadOnly; }
        }

        internal void ResetStatus()
        {
            if (_collection == null)
            {
                return;
            }
            LoadingStatus = LoadingStatus.None;
        }

        /// <summary>
        /// 执行加载数据工厂
        /// </summary>
        /// <param name="loadFactory"></param>
        /// <param name="isReload">是否重新加载</param>
        /// <exception cref="NullReferenceException"></exception>
        internal void OnLoadFactory(Func<bool, bool> loadFactory, bool isReload)
        {
            if (_collection == null)
            {
                return;
            }
            //重新加载或未加载成功时，执行加载数据工厂
            if (isReload || !HasLoadSuccess)
            {
                if (loadFactory != null)
                {
                    LoadingStatus = loadFactory(isReload) ? LoadingStatus.Success : LoadingStatus.Error;
                }
            }
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (_collection != null)
                {
                    _collection.Dispose();
                }
                _collection = null;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~CacheContainer()
        {
            Dispose(false);
        }
    }
}