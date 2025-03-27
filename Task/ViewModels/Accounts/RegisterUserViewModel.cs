using System.ComponentModel.DataAnnotations;

namespace Task.ViewModels.Accounts
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage="*")]
        public string Username { get; set; }

        [Required(ErrorMessage= "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage= "*")]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
