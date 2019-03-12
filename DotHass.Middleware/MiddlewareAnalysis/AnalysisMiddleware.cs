using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotHass.Middleware.MiddlewareAnalysis
{
    public class AnalysisMiddleware
    {
        private readonly DiagnosticSource _diagnostics;

        public AnalysisMiddleware(DiagnosticSource diagnosticSource)
        {
            _diagnostics = diagnosticSource;
        }

        private Func<IDictionary<string, object>, Task> next;

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Guid _instanceId = Guid.NewGuid();
            var _middlewareName = next.Target.GetType().FullName;

            var startTimestamp = Stopwatch.GetTimestamp();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (_diagnostics.IsEnabled("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting"))
            {
                _diagnostics.Write(
                    "Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting",
                    new
                    {
                        name = _middlewareName,
                        environment,
                        instanceId = _instanceId,
                        timestamp = startTimestamp,
                    });
            }

            try
            {
                await next(environment);
                watch.Stop();
                if (_diagnostics.IsEnabled("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished"))
                {
                    var currentTimestamp = Stopwatch.GetTimestamp();
                    _diagnostics.Write(
                        "Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished",
                        new
                        {
                            name = _middlewareName,
                            environment,
                            instanceId = _instanceId,
                            executionTime = watch.ElapsedMilliseconds,
                            timestamp = currentTimestamp,
                            duration = currentTimestamp - startTimestamp,
                        });
                }
            }
            catch (Exception ex)
            {
                if (_diagnostics.IsEnabled("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException"))
                {
                    var currentTimestamp = Stopwatch.GetTimestamp();
                    _diagnostics.Write(
                        "Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException",
                        new
                        {
                            name = _middlewareName,
                            environment,
                            instanceId = _instanceId,
                            timestamp = currentTimestamp,
                            duration = currentTimestamp - startTimestamp,
                            exception = ex,
                        });
                }
                throw;
            }
        }
    }
}