using Store.Models;
using Store.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetProductForStoreById(int counter, List<int> listOfEmptyStockIds, int id);
        Task<List<Product>> GetProductForStores(int counter, List<int> listOfEmptyStockIds);
    }
}
