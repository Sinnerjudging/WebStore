using Microsoft.AspNetCore.Http;
using Store.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class UserFormViewModel
    {
        public IEnumerable<Gender> Genders { get; set; }
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(25)]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(25)]
        public string LastName { get; set; }



        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public int GenderId { get; set; }

        public IFormFile Photo { get; set; }
        public string PhotoPath { get; set; }


        [Display(Name = "Adres 1")]
        public string Address1 { get; set; }

        [Display(Name = "Adres 2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        public string Email { get; set; }



    }
}
