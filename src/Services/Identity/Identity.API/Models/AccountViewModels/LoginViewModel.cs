using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DisplayName("Email")]
        public string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; init; }
        
        public bool RememberMe { get; init; }

        public string ReturnUrl { get; init; }
    }
}
