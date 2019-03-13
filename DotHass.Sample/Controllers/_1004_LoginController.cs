using DotHass.Identity.Services;
using DotHass.Middleware.Authorization;
using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Sample.Model.Identity;
using DotHass.Sample.Protocol;
using DotHass.Sample.Service;
using DotHass.Sample.ValidationModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GYJ.Controllers
{
    [AllowAnonymous]
    public class _1004_LoginController : GameController
    {
        private IIdentityService<GameUser> identityService;


        public _1004_LoginController(IIdentityService<GameUser> identityService, RoleService roleService) : base(roleService)
        {
            this.identityService = identityService;
        }

        public async Task<IActionResult> Handler(PassPortValidationModel model)
        {
            if (this.TryValidateModel(model, out ICollection<ValidationResult> errors) == false)
            {
                return new StatusCodeResult(StatusCodes.Status10000ValidationError);
            }

            bool loginResult = await identityService.Login(model.Pid, model.Pwd, this.Sid);

            if (loginResult == false)
            {
                return new StatusCodeResult(StatusCodes.Status10001RegisterError);
            }

            var user = await identityService.GetUser(model.Pid);
            WriteRoleId(user.GameRoleId);

            //  await _mediator.Send(new RoleLoginCommand(role));

            return new ObjectResult(new UserProtocol()
            {
                UserId = user.Id
            });
        }
    }
}