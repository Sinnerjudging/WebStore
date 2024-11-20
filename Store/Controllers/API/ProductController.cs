using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Services.EF;
using System.Threading.Tasks;

namespace Store.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public ProductController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id.Equals(id));

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return Conflict("Something wrong");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);

        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Product product)
        {
            var productInDb = await _context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);

            if (productInDb == null)
            {
                return Conflict("Something wrong");
            }

            productInDb.CategoryId = product.CategoryId;
            productInDb.Name = product.Name;
            productInDb.Description = product.Description;
            productInDb.Price = product.Price;
            productInDb.SexId = product.SexId;
            productInDb.ColorId = product.ColorId;

            _context.Products.Update(productInDb);
            await _context.SaveChangesAsync();

            return Ok(product);

        }
    }
}