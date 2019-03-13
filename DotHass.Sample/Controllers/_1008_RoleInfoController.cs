using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Sample.Protocol;
using DotHass.Sample.Service;

namespace GYJ.Controllers
{
    public class _1008_RoleInfoController : GameController
    {
        private GangService gangService;

        public _1008_RoleInfoController(RoleService roleService, GangService gang) : base(roleService)
        {
            this.gangService = gang;
        }

        public IActionResult Handler()
        {
            var protocol = new RoleProtocol()
            {
                roleid = Role.Id,
                name = Role.RoleName,
                gold = Role.Gold,
                money = Role.Money,
                exp = Role.Experience,
                gang = gangService.GetById(1)
            };
            return Ok(protocol);
        }
    }
}