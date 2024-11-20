using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Services.EF;
using Store.Services.Interfaces;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class StoreController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IStockService _stockService;
        private readonly IMapper _mapper;

        public StoreController(SignInManager<User> signInManager, 
                               IMapper mapper,
                               IProductService productService,
                               IStockService stockService,
                               IUserService userService)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _productService = productService;
            _stockService = stockService;
            _userService = userService;
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int.TryParse(HttpContext.Request.Cookies["counter"], out int counter);

            var listOfEmptyStockIds = GetListOfEmtyStocksIds(counter);

            var viewModel = await _productService.GetProductForStore(counter, listOfEmptyStockIds);

            return View(viewModel);
        }

        private List<int> GetListOfEmtyStocksIds(int counter)
        {
            var qtyList = new List<int>();


            for (int i = 1; i <= counter; i++)
            {
                var stockId = int.Parse(HttpContext.Request.Cookies["stock-" + i]);

                var qty = int.Parse(HttpContext.Request.Cookies["stock-" + stockId + "-qty"]);

                if (qty == 0)
                {
                    qtyList.Add(stockId);
                }
                else if (qty < 0)
                {
                    throw new Exception("Qty of stockId: " + stockId + " < 0");
                }
            }

            var distinct = qtyList.Distinct().ToList();

            return distinct;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            List<Product> model = new List<Product>();

            int.TryParse(HttpContext.Request.Cookies["counter"], out int counter);

            var listOfEmptyStockIds = GetListOfEmtyStocksIds(counter);

            var viewModel = await _productService.GetProductForStoreById(counter, listOfEmptyStockIds, id);

            model = viewModel.Products;

            return View(model[0]);
        }

        [HttpPost]
        public IActionResult Cart(int id, int counter)
        {
            string cookieName = "stock-" + counter;

            HttpContext.Response.Cookies.Append(cookieName, id.ToString());
            HttpContext.Response.Cookies.Append("counter", counter.ToString());

            var stock = _stockService.GetStockById(id);

            if (stock == null)
            {
                return BadRequest("Stock not found");
            }

            int qty;
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["stock-" + id + "-qty"]))
            {
                qty = stock.Qty;
            }
            else
            {
                qty = int.Parse(HttpContext.Request.Cookies["stock-" + id + "-qty"]);
            }

            qty--;
            HttpContext.Response.Cookies.Append("stock-" + id + "-qty", qty.ToString());
            return Ok();
        }

        public int FindCookieByStockId(int stockId, int counter)
        {
            for (int i = 1; i <= counter; i++)
            {
                if (stockId == int.Parse(HttpContext.Request.Cookies["stock-" + i]))
                {
                    return i;
                }
            }

            throw new Exception("No such cookie found");
        }

        public IActionResult DeleteFromCart(int stockId)
        {
            int.TryParse(HttpContext.Request.Cookies["counter"], out int counter);
            var id = FindCookieByStockId(stockId, counter);

            Replace(id, counter);
            counter--;

            HttpContext.Response.Cookies.Append("counter", counter.ToString());

            int.TryParse(HttpContext.Request.Cookies["stock-" + stockId + "-qty"], out int qty);
            HttpContext.Response.Cookies.Delete("stock-" + stockId + "-qty");
            qty++;
            HttpContext.Response.Cookies.Append("stock-" + stockId + "-qty", qty.ToString());

            return Ok();
        }

        [HttpGet]
        public IActionResult Cart()
        {
            try
            {
                int.TryParse(HttpContext.Request.Cookies["counter"], out int counter);

                if (counter == 0)
                {
                    var model = new List<OrderProduct>();
                    return View("Cart", model);
                }
                else
                {
                    var list = GetOrderProductsList(counter);

                    return View("Cart", list);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string stripeEmail, string stripeToken)
        {
            try
            {
                int.TryParse(HttpContext.Request.Cookies["counter"], out int counter);

                var orderProducts = GetOrderProductsList(counter);

                HttpContext.Response.Cookies.Delete("counter");

                await _userService.Payment(stripeEmail, stripeToken, counter, orderProducts, User);

                return RedirectToAction("OrdersHistory", "Account");
            }
            catch(Exception ex)
            {
                return RedirectToAction("OrdersHistory", "Account");
            }
        }

        private void Replace(int id, int counter)
        {
            var lastElement = HttpContext.Request.Cookies["stock-" + counter];
            HttpContext.Response.Cookies.Append("stock-" + id, lastElement);
            HttpContext.Response.Cookies.Delete("stock-" + counter);
        }

        [HttpGet]
        public IActionResult ShippingInformation()
        {
            var viewModel = new ShippingInformationModel();

            if (_signInManager.IsSignedIn(User))
            {
                var user = _userService.GetUserByUserName(User.Identity.Name);

                viewModel.User = user;

                return View(viewModel);

            }

            viewModel.User = new User();


            return View(viewModel);
        }

        private List<OrderProduct> GetOrderProductsList(int counter)
        {
            var list = new List<OrderProduct>();

            for (int i = 1; i <= counter; i++)
            {
                int stockId;
                int.TryParse(HttpContext.Request.Cookies["stock-" + i], out stockId);

                if (list.Any(x => x.StockId == stockId))
                {
                    var el = list.First(x => x.StockId == stockId).Qty++;
                }
                else
                {
                    var stock = _stockService.GetStockById(stockId);

                    var orderProduct = new OrderProduct
                    {
                        StockId = stockId,
                        ProductId = stock.ProductId,
                        Qty = 1,
                        Stock = stock,
                        Product = _productService.GetById(stock.ProductId)
                    };

                    list.Add(orderProduct);
                }
            }

            return list;
        }

        [HttpPost]
        public async Task<IActionResult> ShippingInformation(ShippingInformationModel model)
        {
            try
            {
                await _userService.UpdateUser(model, User);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}