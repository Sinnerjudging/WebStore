using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Services.EF;
using Store.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class AccountController : Controller
    {

        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public AccountController(StoreContext context, 
            UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostEnvironment, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {

            var viewModel = new RegisterViewModel
            {
                Genders = _context.Genders.ToList()
            };
           
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            var viewModel = new LoginViewModel();

            return View(viewModel);

        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName,
                        model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {

                            return Redirect(returnUrl);
                        }
                        System.Diagnostics.Debug.WriteLine($"Użytkownik {model.UserName} zalogował się.");
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "");

                return View("Login", model);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(x => x.OrderProducts)
                .FirstOrDefaultAsync(x => x.Id == id);

            var orderProducts = order.OrderProducts;
            foreach (var orderProduct in orderProducts)
            {
                _context.Remove(orderProduct);
                var stockInDb = _context.Stock.FirstOrDefault(x => x.Id == orderProduct.StockId);
                if (stockInDb == null)
                {
                    var stock = new Stock
                    {
                        Id = orderProduct.StockId,
                        Name = orderProduct.Stock.Name,
                        ProductId = orderProduct.Stock.ProductId,
                        IsLastElementOrdered = false,
                        Qty = orderProduct.Stock.Qty + 1
                    };
                    await _context.Stock.AddAsync(stock);

                }
                else
                {
                    stockInDb.Qty++;
                    stockInDb.IsLastElementOrdered = false;

                    _context.Stock.Update(stockInDb);
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrdersHistory");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    GenderId = model.GenderId,
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.Phone,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    PostCode = model.PostCode,
                    City = model.City
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            model.Genders = _context.Genders.ToList();

            return View("Register", model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }



        public async Task<IActionResult> Profile()
        {
             _signInManager.IsSignedIn(User);

             var userName = User.Identity.Name;

             var userInDb = await _userManager.FindByNameAsync(userName);

            var user = await _context.Users
                .Include(u => u.Gender)
                .SingleAsync(x => x.Id.Equals(userInDb.Id));


            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            var viewModel = new UserFormViewModel() 
            {
                Id = user.Id,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GenderId = user.GenderId,
                PhoneNumber = user.PhoneNumber,
                PostCode = user.PostCode,
            };

            viewModel.Genders = _context.Genders.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserFormViewModel model)
        {
            string uniqueFileName = null;

            if (ModelState.IsValid)
            {
                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                     uniqueFileName = Guid.NewGuid() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await model.Photo.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }


                var user = await _userManager.FindByIdAsync(model.Id.ToString());

                user.Id = model.Id;
                user.Address1 = model.Address1;
                user.Address2 = model.Address2;
                user.City = model.City;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.GenderId = model.GenderId;
                user.PhoneNumber = model.PhoneNumber;
                user.PostCode = model.PostCode;
                user.PhotoPath = uniqueFileName;

                if (model.Photo != null)
                {
                    user.PhotoPath = uniqueFileName;
                }

                await _userManager.UpdateAsync(user);

                await _context.SaveChangesAsync();
                return Ok();
            }


            var viewModel = new UserFormViewModel()
            {
                Id = model.Id,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                GenderId = model.GenderId,
                PhoneNumber = model.PhoneNumber,
                PostCode = model.PostCode,
                Photo = model.Photo,
                PhotoPath = model.PhotoPath
            };

            viewModel.Genders = _context.Genders.ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> OrdersHistory()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var orders = _context.Orders.Where(x=>x.UserId == user.Id);

            return View(orders);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public IActionResult News()
        {
            return View();
        }

        [HttpGet]
        public IActionResult KoiFood()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderProducts)
                .ThenInclude(x => x.Stock)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);


            return View(order);
        }
    }
}