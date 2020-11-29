 using System.ComponentModel;
 using System.ComponentModel.DataAnnotations;

 namespace Identity.API.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [MaxLength(32)]
        public string UserName { get; init; }

        [Required]
        [MaxLength(32)]
        public string FirstName { get; init; }

        [Required]
        [MaxLength(32)]
        public string LastName { get; init; }

        [Required]
        public string Country { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(64)]
        [RegularExpression(@"^(?=(.*\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{6,}$",
            ErrorMessage = "Password must have at least one capital letter, number and special sign.")]
        public string Password { get; init; }

        [Required]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; }

        public string ReturnUrl { get; init; }
    }
}
