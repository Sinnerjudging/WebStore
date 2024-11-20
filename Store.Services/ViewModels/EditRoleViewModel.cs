using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Store.Models;

namespace Store.ViewModels
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<User> Users { get; set; }
    }
}
