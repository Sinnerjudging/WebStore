using KStore.Models;
using Store.Models;

namespace Store.Repositories.Entities
{
    public class AppUser : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
