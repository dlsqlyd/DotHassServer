using DotHass.Middleware.Authorization;
using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Sample.Protocol;
using DotHass.Sample.Service;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GYJ.Controllers
{
    [AllowAnonymous]
    public class _10000_TestController : GameController
    {
        private ILogger<_10000_TestController> logger;

        public _10000_TestController(ILogger<_10000_TestController> logger, RoleService roleService) : base(roleService)
        {
            this.logger = logger;
        }

        public Task<IActionResult> Handler(string aaaa)
        {
            this.logger.LogInformation($"aaaa的值是{aaaa}");

            var r = new ObjectResult(new UserProtocol()
            {
                UserId = "5555555555555555555555",
            });

            return Task.FromResult<IActionResult>(r);
        }
    }
}