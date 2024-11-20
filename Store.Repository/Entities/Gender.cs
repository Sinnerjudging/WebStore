using KStore.Models;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Gender : BaseEntity
    {
       
        [Required]
        public string Name { get; set; }
    }
}
