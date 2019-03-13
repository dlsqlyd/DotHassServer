using DotHass.Identity.Services;
using DotHass.Middleware.Authorization;
using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Sample.Model.Data;
using DotHass.Sample.Model.Identity;
using DotHass.Sample.Protocol;
using DotHass.Sample.Service;
using DotHass.Sample.ValidationModel;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GYJ.Controllers
{
    [AllowAnonymous]
    public class _1005_RegisterController : GameController
    {
        private ILogger<_1005_RegisterController> _logging;
        private IIdentityService<GameUser> identityService;

        public _1005_RegisterController(ILogger<_1005_RegisterController> logging, IIdentityService<GameUser> identityService, RoleService roleService) : base(roleService)
        {
            this._logging = logging;
            this.identityService = identityService;
        }

        public async Task<IActionResult> Handler(PassPortValidationModel model)
        {
            if (this.TryValidateModel(model, out ICollection<ValidationResult> errors) == false)
            {
                return Error(StatusCodes.Status10000ValidationError);
            }

            //检查用户是否存在
            var user = await identityService.GetUser(model.Pid);
            if (user != null)
            {
                return Error(StatusCodes.Status10002UserExist);
            }

            //注册用户
            bool result = await identityService.Register(model.Pid, model.Pwd);
            if (result == false)
            {
                return Error(StatusCodes.Status10001RegisterError);
            }
            user = await identityService.GetUser(model.Pid);

            //认证
            await identityService.SignInAsync(user, this.Sid);

            //注册用户命令
            GameRole role = roleService.CreateRole(user);
            user.GameRoleId = role.Id;

            WriteRoleId(role.Id);
            identityService.Update(user);

            //todo:根据Retail获取Retaildata
            return Ok(new UserProtocol()
            {
                UserId = user.Id
            });
        }
    }
}