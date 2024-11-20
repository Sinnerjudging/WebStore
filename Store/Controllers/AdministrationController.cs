using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Services.EF;
using Store.Services.Interfaces;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Controllers
{
    [Authorize(Roles ="Admin")]

    public class AdministrationController : Controller
    {

        private readonly IRoleService _roleService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        private readonly StoreContext _context;

        public AdministrationController(IRoleService roleService, UserManager<User> userManager, StoreContext context,
            IWebHostEnvironment hostEnvironment, IMapper mapper, SignInManager<User> signInManager)
        {
            _roleService = roleService;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();

            if (_signInManager.IsSignedIn(User))
            {
                var userName = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);

                users.Remove(user);
            }

            return View(users);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View(new CreateRoleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _roleService.CreateRole(model);


                if (result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"Roles have been created {model.RoleName}");

                    return RedirectToAction("Roles");
                }

                foreach (var error in result.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed attempt to create a role {model.RoleName}");

                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleService.GetRoles();

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            try
            {
                var result = await _roleService.GetRoleById(id);

                return View(result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            try
            {
                var result = await _roleService.EditRole(model);

                return View(result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(int roleId)
        {
            try
            {
                ViewBag.roleId = roleId;

                var model = await _roleService.EditUsersInRole(roleId);

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, int roleId)
        {
            try
            {
                var result = await _roleService.UpdateUserInRole(model, roleId);

                return RedirectToAction("EditRole", new { id = roleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Gender)
                .SingleAsync(x => x.Id.Equals(id));

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users
                .Include(x => x.Gender)
                .SingleOrDefaultAsync(x => x.Id.Equals(id));

            if (user == null)
                return NotFound();

            var viewModel = new UserFormViewModel
            {
                Genders = _context.Genders.ToList(),
                GenderId = user.GenderId,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                PhotoPath = user.PhotoPath
            };

            return View("EditUser", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserFormViewModel model)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await model.Photo.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }


                var user = _userManager.FindByIdAsync(model.Id.ToString()).Result;

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


                await _userManager.UpdateAsync(user);

                _context.SaveChanges();

                return RedirectToAction("Users", "Administration");
            }

            var viewModel = new UserFormViewModel
            {
                Genders = _context.Genders.ToList(),
                GenderId = model.GenderId,
                PhoneNumber = model.PhoneNumber,
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName
            };

            return View("EditUser", viewModel);
        }


        [HttpGet]
        public IActionResult Products()
        {
            var products = _context.Products
               .Include(u => u.Color)
               .Include(u => u.Sex)
               .Include(u => u.Category)
               .ToList();

            return View(products);
        }


        [HttpGet]
        public IActionResult AddProduct()
        {
            var productFormViewModel = new ProductFormViewModel
            {
                Categories = _context.Categories.ToList(),
                Colors = _context.Colors.ToList(),
                Sexes = _context.Sexes.ToList()
            };

            return View("ProductForm", productFormViewModel);
        }

        [HttpGet]
        public IActionResult Stock(int id)
        {
            var stock = _context.Stock.FirstOrDefault(x => x.ProductId == id);

            if (stock == null)
            {
                return View(new StockViewModel { Stock = new List<Stock>(), ProductId = id });
            }

            var viewModel = new StockViewModel
            {
                ProductId = id,
                Name = stock.Name,
                Qty = stock.Qty,
                Stock = _context.Stock.Where(x => x.ProductId == id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Stock(StockViewModel model)
        {
            if (ModelState.IsValid)
            {
                var stock = new Stock();
                stock.Name = model.Name;
                stock.ProductId = model.ProductId;
                stock.Qty = model.Qty;
                if (model.Qty == 0)
                {
                    stock.IsLastElementOrdered = true;
                }

                if (model.Id == 0)
                {
                    await _context.Stock.AddAsync(stock);
                }
                else
                {
                    stock.Id = model.Id;

                    _context.Stock.Update(stock);
                }
                await _context.SaveChangesAsync();

                return Ok();
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStock(int id)
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);

            if (stock == null)
            {
                return NotFound();
            }


            return View(stock);
        }

        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);

            if (stock == null)
            {
                return NotFound();
            }

            _context.Remove(stock);
            await _context.SaveChangesAsync();


            return RedirectToAction("Stock", new { id = stock.ProductId });
        }

        [HttpGet]
        public IActionResult AddColor()
        {
            return View("ColorForm", new Color());
        }

        [HttpGet]
        public async Task<IActionResult> EditColor(int id)
        {
            var color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            return View("ColorForm", color);
        }

        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (color == null)
            {
                return NotFound();
            }

            _context.Remove(color);
            await _context.SaveChangesAsync();

            return RedirectToAction("Color");
        }


        [HttpGet]
        public IActionResult Color()
        {
            var colors = _context.Colors.ToList();

            return View(colors);
        }

        [HttpGet]
        public IActionResult Category()
        {
            return View(_context.Categories.ToList());
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View("CategoryForm", new Category());
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);

            return View("CategoryForm", category);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _context.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Category");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductFormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var product = new Product();

                    product.Id = model.Id;
                    product.CategoryId = model.CategoryId;
                    product.Description = model.Description;
                    product.Price = decimal.Parse(model.Price);
                    product.Name = model.Name;
                    product.ColorId = model.ColorId;
                    product.SexId = model.SexId;

                    string uniqueFileName = null;

                    if (model.Photo != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }

                    product.PhotoPath = uniqueFileName;
                    if (product.Id != 0)
                    {
                        _context.Products.Update(product);
                    }
                    else
                    {
                       
                        await _context.Products.AddAsync(product);
                    }

                    product.PhotoPath = uniqueFileName;

                    await _context.SaveChangesAsync();


                    return RedirectToAction("Index");

                }

                var viewModel = new ProductFormViewModel
                {
                    Description = model.Description,
                    Name = model.Name,
                    ColorId = model.ColorId,
                    Price = model.Price,
                    SexId = model.SexId,
                    CategoryId = model.CategoryId,
                    Categories = _context.Categories.ToList(),
                    Colors = _context.Colors.ToList(),
                    Sexes = _context.Sexes.ToList()
                };

                return View("AddProduct", viewModel);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var productInDb = _context.Products.FirstOrDefault(x => x.Id.Equals(id));

            if (productInDb == null)
            {
                return NotFound();
            }

            var viewModel = new ProductFormViewModel();

            viewModel.CategoryId = productInDb.CategoryId;
            viewModel.Name = productInDb.Name;
            viewModel.Description = productInDb.Description;
            viewModel.Price = productInDb.Price.ToString();
            viewModel.SexId = productInDb.SexId;
            viewModel.ColorId = productInDb.ColorId;
            viewModel.Id = productInDb.Id;

            viewModel.Categories = _context.Categories.ToList();
            viewModel.Colors = _context.Colors.ToList();
            viewModel.Sexes = _context.Sexes.ToList();


            return View("ProductForm", viewModel);

        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productInDb = await _context.Products.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (productInDb == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productInDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    _context.Categories.Update(model);
                }
                else
                {
                    var category = new Category
                    {
                        Name = model.Name
                    };
                    _context.Categories.Add(category);

                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Category");

            }

            var viewModel = new Category { Name = model.Name };
            return View("AddCategory", viewModel);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveColor(Color model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    _context.Update(model);

                }
                else
                {
                    var color = new Color
                    {
                        Name = model.Name
                    };

                    _context.Colors.Add(color);
                }

                await _context.SaveChangesAsync();
                
                return RedirectToAction("Color");
            }


            var viewModel = new Color
            {
                Name = model.Name,
            };
            return View("ColorForm", viewModel);
        }

        [HttpGet]
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderProducts)
                .ThenInclude(x => x.Stock)
                .ThenInclude(x => x.Product)
                .ToList();


            return View(orders);
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

            return RedirectToAction("Orders");
        }


        public async Task<IActionResult> SentOrder(int id)
        {
            var order = await _context.Orders
               .FirstOrDefaultAsync(x => x.Id == id);

            order.IsSend = true;
            order.OrderSendDate = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
    }
}