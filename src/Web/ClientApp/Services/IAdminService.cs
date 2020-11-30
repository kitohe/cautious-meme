using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface IAdminService
    {
        Task<string> TestMessagingApiEndpoint();
    }
}
