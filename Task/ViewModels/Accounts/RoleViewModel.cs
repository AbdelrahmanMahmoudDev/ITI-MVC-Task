using System.ComponentModel.DataAnnotations;

namespace Task.ViewModels.Accounts
{
    public class RoleViewModel
    {
        [Required(ErrorMessage="*")]
        public string RoleName { get; set; }
    }
}
