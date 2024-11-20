using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class LoginViewModel
    {

        [Required]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
