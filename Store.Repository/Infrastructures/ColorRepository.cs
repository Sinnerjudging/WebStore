using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructure
{
    public class ColorRepository : RepositoryBase<Color>, IColorRepository
    {
        public ColorRepository(StoreContext context) : base(context)
        {
        }
    }
}
