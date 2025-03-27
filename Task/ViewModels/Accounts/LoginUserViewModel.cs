using System.ComponentModel.DataAnnotations;

namespace Task.ViewModels.Accounts
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "*")]
        public string Username { get; set; }
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }
        [Required(ErrorMessage= "*")]
        public bool IsRemembered { get; set; }
    }
}
