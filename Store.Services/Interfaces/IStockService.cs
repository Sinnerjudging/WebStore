using Store.Models;

namespace Store.Services.Interfaces
{
    public interface IStockService
    {
        Stock GetStockById(int id);
    }
}
