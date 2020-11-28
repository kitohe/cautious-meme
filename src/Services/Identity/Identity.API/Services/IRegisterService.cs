using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.API.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Services
{
    public interface IRegisterService
    {
        Task<IEnumerable<IdentityError>> RegisterUser(RegisterViewModel model);
    }
}
