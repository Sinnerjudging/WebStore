using Microsoft.AspNetCore.Identity;
using Store.Models;
using Store.Services.EF;
using Store.Services.Interfaces;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Services.Infrastructues
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly StoreContext _context;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager, StoreContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IdentityResult> CreateRole(CreateRoleViewModel model)
        {
            var role = new Role
            {
                Name = model.RoleName
            };

            var result = await _roleManager.CreateAsync(role);

            return result;
        }

        public async Task<EditRoleViewModel> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());

            if (role == null)
            {
                throw new  Exception($"Role with Id = {model.Id} cannot be found");
            }

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new  Exception($"Error when update role");
            }

            model.Users = new List<User>();

            foreach (var user in await _userManager.GetUsersInRoleAsync(role.Name))
            {
                model.Users.Add(user);
            }

            return model;
        }
        private async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            foreach (var userInRole in usersInRole)
            {
                if (userInRole.Id == user.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<UserRoleViewModel>> EditUsersInRole(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                throw new Exception($"Role with Id = {roleId} cannot be found");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await IsInRoleAsync(user, role.Name)
                };

                model.Add(userRoleViewModel);
            }

            return model;
        }

        public async Task<EditRoleViewModel> GetRoleById(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
            {
                throw new Exception("Not found");
            }

            var model = new EditRoleViewModel
            {
                Id = id,
                RoleName = role.Name,
                Users = new List<User>()
            };

            foreach (var user in await _userManager.GetUsersInRoleAsync(role.Name))
            {
                model.Users.Add(user);
            }

            return model;
        }

        public List<Role> GetRoles()
        {
            var roles = _roleManager.Roles;

            return roles.ToList();
        }

        private async Task<bool> DeleteRoleAsync(User user, int roleId)
        {

            var userRole = new IdentityUserRole<int>
            {
                UserId = user.Id,
                RoleId = roleId
            };

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();


            return true;
        }

        public async Task<bool> UpdateUserInRole(List<UserRoleViewModel> model, int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                throw new Exception($"Role with Id = {roleId} cannot be found");
            }

            bool ifSucceeded = false;

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId.ToString());

                if (model[i].IsSelected && !(await IsInRoleAsync(user, role.Name)))
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        ifSucceeded = true;
                    }
                }
                else if (!model[i].IsSelected && await IsInRoleAsync(user, role.Name))
                {
                    ifSucceeded = await DeleteRoleAsync(user, role.Id);
                }

                if (ifSucceeded)
                {
                    if (i < model.Count - 1)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return true;
        }
    }
}
