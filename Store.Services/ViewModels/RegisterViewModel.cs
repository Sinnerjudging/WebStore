using Store.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class RegisterViewModel
    {
        public IEnumerable<Gender> Genders { get; set; }
      
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [PhoneAttribute]
        [Required]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Login")]
        [MinLength(5)]
        [MaxLength(15)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Passwords vary")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public int GenderId { get; set; }

        [Display(Name = "Adres 1")]
        public string Address1 { get; set; }

        [Display(Name = "Adres 2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

    }
}
