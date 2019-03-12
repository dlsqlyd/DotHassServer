using System;
using System.Collections.Generic;

namespace DotHass.Repository.Aspects
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public abstract class RepositoryActionBaseAttribute : Attribute
    {
        protected RepositoryActionBaseAttribute()
        {
            Enabled = true;
            Order = 9999; // high number so if they don't set it, it happens last
        }

        public int Order { get; set; }
        public bool Enabled { get; set; }

        public virtual void OnInitialized<T>(RepositoryActionContext<T> context) where T : class
        {
        }

        public virtual void OnError(Exception ex)
        {
        }

        public virtual bool OnAddExecuting<T>(T entity) where T : class
        {
            return true;
        }

        public virtual void OnAddExecuted<T>(T entity) where T : class
        {
        }

        public virtual bool OnAddRangeExecuting<T>(IEnumerable<T> entitys) where T : class
        {
            return true;
        }

        public virtual void OnAddRangeExecuted<T>(IEnumerable<T> entitys) where T : class
        {
        }

        public virtual bool OnUpdateExecuting<T>(T entity) where T : class
        {
            return true;
        }

        public virtual void OnUpdateExecuted<T>(T entity) where T : class
        {
        }

        public virtual bool OnDeleteExecuting<T>(T entity) where T : class
        {
            return true;
        }

        public virtual void OnDeleteExecuted<T>(T entity) where T : class
        {
        }

        public virtual T OnFindExecuting<T>(params object[] keyValues) where T : class
        {
            return default;
        }

        public virtual void OnFindExecuted<T>(T entity) where T : class
        {
        }

        public virtual IEnumerable<T> OnFindAllExecuting<T>(params object[] keyValues) where T : class
        {
            return default;
        }

        public virtual void OnFindAllExecuted<T>(IEnumerable<T> entity) where T : class
        {
        }
    }
}