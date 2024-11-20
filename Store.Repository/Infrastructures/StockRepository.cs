using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructure
{
    public class StockRepository : RepositoryBase<Stock>, IStockRepository
    {
        public StockRepository(StoreContext context) : base(context)
        {
        }
    }
}
