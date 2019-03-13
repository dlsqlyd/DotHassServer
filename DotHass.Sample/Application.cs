using DotHass.Middleware.Authentication;
using DotHass.Middleware.Authorization;
using DotHass.Middleware.ExceptionHandler;
using DotHass.Middleware.Mvc.Controller;
using DotHass.Middleware.Session;
using DotHass.Sample.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Sample
{
    public class Application : IHostedService
    {
        private IServiceProvider provider;

        public Application(IServiceProvider provider, IApplicationLifetime applicationLifetime)
        {
            this.provider = provider;
            applicationLifetime.ApplicationStarted.Register(this.Started);
            applicationLifetime.ApplicationStopping.Register(this.Stopping);
            applicationLifetime.ApplicationStopped.Register(this.Stopped);
        }

        public void Started()
        {
            //初始化依赖注入中的大量的类..避免第一次请求会缓慢的问题
            provider.GetService<AuthenticationMiddleware>();
            provider.GetService<AuthorizationMiddleware>();
            provider.GetService<ExceptionHandlerMiddleware>();
            provider.GetService<SessionMiddleware>();
            provider.GetService<ControllerMiddleware>();

            //初始化数据库连接-避免第一次请求慢-正式项目不要这样..随便请求一次数据库
            var roleservice = provider.GetService<RoleService>();
            roleservice.dataRepos.FindAll();

            var cache = provider.GetService<IDistributedCache>();
            cache.Refresh("test");
        }

        public void Stopping()
        {
        }

        public void Stopped()
        {
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}