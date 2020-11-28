namespace Identity.API.Models.AccountViewModels
{
    public class LoginViewModel
    {
        public string Email { get; init; }

        public string Password { get; init; }
        
        public bool RememberMe { get; init; }

        public string ReturnUrl { get; init; }
    }
}
