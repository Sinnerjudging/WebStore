using KStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Order : BaseEntity
    {
        public string ChargeId { get; set; }

        public bool IsSend { get; set; } = false;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/DD/YYYY}")]
        [Required]
        public DateTime? OrderDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/DD/YYYY}")]
        public DateTime? OrderSendDate { get; set; } = null;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Adres 1")]
        public string Address1 { get; set; }

        [Display(Name = "Adres 2")]
        public string Address2 { get; set; }

        public string Email { get; set; }

        [Required]
        [Display(Name ="City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderProduct> OrderProducts{ get; set; }

    }
}
