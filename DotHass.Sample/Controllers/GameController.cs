using DotHass.Middleware.Mvc.Controller;
using DotHass.Middleware.Session;
using DotHass.Sample.Model.Data;
using DotHass.Sample.Service;

namespace GYJ.Controllers
{
    public abstract class GameController : BaseController
    {
        protected RoleService roleService;

        public const string ROLEID_SESSION_KEY = "roleid";

        public GameController(RoleService roleService)
        {
            this.roleService = roleService;
        }

        public long? RoleId
        {
            get
            {
                return Session?.GetLong(ROLEID_SESSION_KEY);
            }
        }

        private GameRole _role;

        public GameRole Role
        {
            get
            {
                if (_role == null)
                {
                    if (RoleId == null)
                    {
                        return null;
                    }
                    _role = roleService.GetRole(RoleId.Value);
                }
                return _role;
            }
            set
            {
                _role = value;
            }
        }

        public void WriteRoleId(long roleId)
        {
            Session?.SetLong(ROLEID_SESSION_KEY, roleId);
        }
    }
}