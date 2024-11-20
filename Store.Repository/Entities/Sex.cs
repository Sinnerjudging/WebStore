using KStore.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Sex : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
