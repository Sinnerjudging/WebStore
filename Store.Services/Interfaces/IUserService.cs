using Store.Models;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Interfaces
{
    public interface IUserService
    {
        Task Payment(string stripeEmail, string stripeToken,int counter, List<OrderProduct> products, ClaimsPrincipal user);
        User GetUserById(int id);
        User GetUserByUserName(string userName);
        Task<bool> UpdateUser(ShippingInformationModel model, ClaimsPrincipal user);
        Task<User> GetUserDetail(int id);
        Task<UserFormViewModel> UpdateUser(UserFormViewModel model, string uniqueFileName);
    }
}
