using System.Threading.Tasks;
using Identity.API.Models.AccountModels;

namespace Identity.API.Services
{
    public interface ILoginService<in T>
    {
        Task<string> SignInAsync(LoginModel model);
    }
}
