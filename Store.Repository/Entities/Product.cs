using KStore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class Product : BaseEntity
    {
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public Category Category{ get; set; }

        [Required]
        [Display(Name="Name")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string PhotoPath { get; set; }

        
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(15,2)")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Display(Name = "Color")]
        public Color Color { get; set; }

        [Display(Name = "Color")]
        public int? ColorId { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public int SexId { get; set; }
        
        [Display(Name = "Gender")]
        public Sex Sex { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [NotMapped]
        public IFormFile Photo{ get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
