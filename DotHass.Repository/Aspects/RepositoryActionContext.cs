using System;

namespace DotHass.Repository.Aspects
{
    public class RepositoryActionContext<T> where T : class
    {
        public RepositoryActionContext(IServiceProvider service)
        {
            Services = service;
        }

        public IServiceProvider Services { get; internal set; }
    }
}