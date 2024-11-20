using Microsoft.AspNetCore.Http;
using Store.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class ProductFormViewModel
    {
        public List<Category> Categories { get; set; }
        //public List<Brand> Brands { get; set; }
        public List<Color> Colors { get; set; }
        public List<Sex> Sexes { get; set; }




        [Display(Name = "Image")]
        public IFormFile Photo { get; set; }

        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }


        public int Id { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "No type selected")]
        public int CategoryId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "No name given")]
        public string Name { get; set; }


        [Display(Name = "Price")]
        [Required(ErrorMessage = "No price given")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [RegularExpression("([0-9,]+)", ErrorMessage = "The price must be a number. Use , instead.")]

        public string Price { get; set; }

        [Required(ErrorMessage = "No color selected. If it is not on the list, add it.")]
        [Display(Name = "Color")]
        public int? ColorId { get; set; }

        //[Required(ErrorMessage = "No brand selected. If it is not on the list, add it.")]
        //[Display(Name = "Brand")]
        //public int? BrandId { get; set; }

        [Display(Name = "Sex")]
        [Required(ErrorMessage = "Gender not selected")]
        public int SexId { get; set; }

        [Display(Name = "Option")]
        public string Description { get; set; }
    }
}