using Store.Repositories.Entities;
using Store.Repositories.Infrastructure;
using Store.Repositories.Interfaces;
using Store.Services.EF;

namespace Store.Repositories.Infrastructures
{
    public class UserRepository : RepositoryBase<AppUser>, IUserRepository
    {
        public UserRepository(StoreContext context) : base(context)
        {
        }
    }
}
