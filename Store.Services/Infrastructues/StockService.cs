using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.Interfaces;
using System.Threading.Tasks;

namespace Store.Services.Infrastructure
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockService;

        public StockService(IStockRepository stockService)
        {
            _stockService = stockService;
        }
        public Stock GetStockById(int id)
        {
            return _stockService.FirstOrDefault(x => x.Id == id);
        }
    }
}
