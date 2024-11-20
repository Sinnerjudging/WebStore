using Store.Models;
using System.Collections.Generic;

namespace Store.ViewModels
{
    public class StoreViewModel
    {
        public List<Product> Products { get; set; }

        public int StockId { get; set; }
        public int Counter { get; set; }
    }
}
