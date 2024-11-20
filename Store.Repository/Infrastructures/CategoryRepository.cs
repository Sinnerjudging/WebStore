using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructure
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(StoreContext context) : base(context)
        {
        }
    }
}
