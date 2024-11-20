using Store.Models;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Interfaces
{
    public interface IProductService
    {
        Task<StoreViewModel> GetProductForStore(int counter, List<int> listOfEmptyStockIds);
        Task<StoreViewModel> GetProductForStoreById(int counter, List<int> listOfEmptyStockIds, int id);
        Product GetById(int id);
    }
}
