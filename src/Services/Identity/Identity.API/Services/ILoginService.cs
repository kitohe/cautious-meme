using System.Threading.Tasks;
using Identity.API.Models.AccountViewModels;
using IdentityServer4.Models;

namespace Identity.API.Services
{
    public interface ILoginService<in T>
    {
        Task<string> LoginAsync(LoginViewModel viewModel);
        Task<LogoutRequest> GetLogoutContextAsync(string logoutId);
    }
}
