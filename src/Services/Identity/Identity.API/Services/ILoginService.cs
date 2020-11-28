using System.Threading.Tasks;
using Identity.API.Models.AccountViewModels;

namespace Identity.API.Services
{
    public interface ILoginService<in T>
    {
        Task<string> LoginAsync(LoginViewModel viewModel);
    }
}
