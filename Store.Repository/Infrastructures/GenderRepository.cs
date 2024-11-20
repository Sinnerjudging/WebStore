using Store.Models;
using Store.Repositories.Infrastructure;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructures
{
    public class GenderRepository : RepositoryBase<Gender>, IGenderRepository
    {
        public GenderRepository(StoreContext context) : base(context)
        {
        }
    }
}
