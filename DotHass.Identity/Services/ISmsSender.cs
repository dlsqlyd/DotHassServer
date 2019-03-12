using System.Threading.Tasks;

namespace DotHass.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}