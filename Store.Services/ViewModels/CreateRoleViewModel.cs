using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string RoleName { get; set; }
    }
}
