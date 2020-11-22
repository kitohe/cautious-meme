using System.Threading.Tasks;
using Identity.API.Models.AccountViewModels;

namespace Identity.API.Services
{
    public interface IRegisterService
    {
        Task RegisterUser(RegisterModel model);
    }
}
