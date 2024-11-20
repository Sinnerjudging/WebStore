using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructure
{
    public class OrderProductRepository : RepositoryBase<OrderProduct>, IOrderProductRepository
    {
        public OrderProductRepository(StoreContext context) : base(context)
        {
        }
    }
}
