using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;
using Store.Services.Interfaces;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Services.Infrastructues
{
    public class UserService : IUserService
    {
        private readonly StoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IGenderRepository _genderRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IOrderRepository _orderRepository;

        public UserService(UserManager<User> userManager, 
                           IUserRepository userRepository,
                           IStockRepository stockRepository,
                           IGenderRepository genderRepository,
                           StoreContext context,
                           IOrderProductRepository orderProductRepository,
                           IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _stockRepository = stockRepository;
            _orderProductRepository = orderProductRepository;
            _orderRepository = orderRepository;
            _genderRepository = genderRepository;
            _context = context;
        }

        public User GetUserById(int id)
        {
            var user = _userRepository.FirstOrDefault(x=>x.User.Id == id,null, "User");

            if(user == null)
            {
                throw new Exception("User not found");
            }

            return user.User;
        }

        public User GetUserByUserName(string userName)
        {
            var user = _userRepository.FirstOrDefault(x => x.User.UserName == userName, null, "User");

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user.User;
        }

        public async Task<User> GetUserDetail(int id)
        {
            var user = await _userRepository.QueryAndSelectAsync(x=> new User() 
                                                                         {
                                                                            Id = x.User.Id,
                                                                            Address1 = x.User.Address1,
                                                                            FirstName = x.User.FirstName,
                                                                            LastName = x.User.LastName,
                                                                            Address2 = x.User.Address2,
                                                                            PhoneNumber = x.User.PhoneNumber,
                                                                            City = x.User.City,
                                                                            Email = x.User.Email,
                                                                            GenderId = x.User.GenderId,
                                                                            Gender = x.User.Gender, 
                                                                            PostCode = x.User.PostCode,
                                                                         },
                                                                 x => x.User.Id == id, null, "User");

            if(user == null ) 
            {
                throw new Exception("Data is null");
            }

            return user.FirstOrDefault();
        }

        public async Task Payment(string stripeEmail, string stripeToken, int counter, List<OrderProduct> orderProducts, ClaimsPrincipal user)
        {
            var customers = new Stripe.CustomerService();
            var charges = new Stripe.ChargeService();

            var userLogin = await _userManager.GetUserAsync(user);

            decimal price = 0;

            foreach (var item in orderProducts)
            {
                var stock = _stockRepository.FirstOrDefault(x=>x.Id == item.StockId);

                stock.Qty -= item.Qty;
                for (int i = 0; i < item.Qty; i++)
                {
                    price += item.Product.Price;
                }

                if (stock.Qty == 0)
                {
                    stock.IsLastElementOrdered = true;
                }

                await _stockRepository.Update(stock);
            }

            var customer = customers.Create(new Stripe.CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken,
                Name = userLogin.FirstName + " " + userLogin.LastName,
                Address = new Stripe.AddressOptions { City = userLogin.City, Country = "Poland", PostalCode = userLogin.PostCode, Line1 = userLogin.Address1, Line2 = userLogin.Address2 },
                Phone = userLogin.PhoneNumber,
            });

            var charge = charges.Create(new Stripe.ChargeCreateOptions
            {
                Amount = Convert.ToInt64(price * 100),
                Description = "Sample Charge",
                Currency = "PLN",
                Customer = customer.Id,
                ReceiptEmail = stripeEmail,
            });

            var order = new Order
            {
                FirstName = userLogin.FirstName,
                LastName = userLogin.LastName,
                Email = userLogin.Email,
                Address1 = userLogin.Address1,
                Address2 = userLogin.Address2,
                City = userLogin.City,
                PostCode = userLogin.PostCode,
                OrderProducts = orderProducts,
                OrderDate = DateTime.Now,
                UserId = userLogin.Id,
                ChargeId = charge.Id
            };

            await _orderRepository.Add(order);

            foreach(var item in orderProducts)
            {
                item.OrderId = order.Id;

                await _orderProductRepository.Add(item);
            }
        }

        public async Task<bool> UpdateUser(ShippingInformationModel model, ClaimsPrincipal user)
        {
            var userLogin = _userManager.GetUserAsync(user).Result;

            userLogin.Address1 = model.Order.Address1;
            userLogin.Address2 = model.Order.Address2;
            userLogin.City = model.Order.City;
            userLogin.FirstName = model.Order.FirstName;
            userLogin.LastName = model.Order.LastName;
            userLogin.PostCode = model.Order.PostCode;

            await _userManager.UpdateAsync(userLogin);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserFormViewModel> UpdateUser(UserFormViewModel model, string uniqueFileName)
        {
            var user = _userManager.FindByIdAsync(model.Id.ToString()).Result;

            user.City = model.City;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.GenderId = model.GenderId;
            user.PhoneNumber = model.PhoneNumber;
            user.PhotoPath = uniqueFileName;

            await _userManager.UpdateAsync(user);

            await _context.SaveChangesAsync();


            return model;
        }
    }
}
