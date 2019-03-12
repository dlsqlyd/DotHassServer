using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Tools.ExcelExport
{
    public class ApplicationHostService : IHostedService
    {
        private AppSettings setting;
        private IApplicationLifetime _appLifetime;

        public ApplicationHostService(IOptions<AppSettings> options, IApplicationLifetime appLifetime)
        {
            setting = options.Value;

            this._appLifetime = appLifetime;

            Console.WriteLine("Root:{0}", setting.Root);
            Console.WriteLine("ExcelPath:{0}", setting.ExcelPath);
            Console.WriteLine("JsonPath:{0}", setting.JsonPath);
            Console.WriteLine("SQLPath:{0}", setting.SQLPath);
            Console.WriteLine("CSharpPath:{0}", setting.CSharpPath);

            if (string.IsNullOrEmpty(setting.ExcelPath))
            {
                throw new Exception("excel 目录配置错误,请检查传入的参数是否正确");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 主机已完全启动。
        /// </summary>
        private void OnStarted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var manager = new ExportManager(setting);
            manager.Export();

            if (setting.ShowInfo == false)
            {
                _appLifetime.StopApplication();
            }

            // Perform post-startup activities here
        }

        /// <summary>
        /// 主机正在执行正常关闭。 仍在处理请求。 关闭受到阻止，直到完成此事件。
        /// </summary>
        private void OnStopping()
        {
            // Perform on-stopping activities here
        }

        /// <summary>
        /// 主机正在完成正常关闭。 应处理所有请求。 关闭受到阻止，直到完成此事件。
        /// </summary>
        private void OnStopped()
        {
            // Perform post-stopped activities here
        }
    }
}