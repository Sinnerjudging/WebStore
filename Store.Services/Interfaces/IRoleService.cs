using Microsoft.AspNetCore.Identity;
using Store.Models;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateRole(CreateRoleViewModel model);
        List<Role> GetRoles();
        Task<EditRoleViewModel> GetRoleById(int id);
        Task<EditRoleViewModel> EditRole(EditRoleViewModel model);
        Task<List<UserRoleViewModel>> EditUsersInRole(int roleId);
        Task<bool> UpdateUserInRole(List<UserRoleViewModel> model, int roleId);
    }
}
