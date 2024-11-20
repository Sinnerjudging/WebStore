using KStore.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Color : BaseEntity
    {
        [Required]
        [Display(Name="Name")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
