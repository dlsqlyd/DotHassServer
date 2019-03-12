using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DotHass.Identity.Services
{
    public interface IIdentityService<TUser> where TUser : IdentityUser
    {
        Task<TUser> GetUser(string name);

        Task SignInAsync(TUser user, string sid);

        Task<bool> Login(string name, string password, string sid);

        Task<bool> Register(string name, string password);

        Task<bool> TwoFactorRegister(string tokenProvider, string tokenID, string tokenCode, string password);

        Task<bool> TwoFactorSendCode(string tokenProvider, string tokenID);

        void Update(TUser user);
    }
}