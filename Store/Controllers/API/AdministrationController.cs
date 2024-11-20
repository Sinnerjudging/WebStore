using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Services.EF;
using System.Linq;

namespace Store.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;


        public AdministrationController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            var userInDb = _context.Users.SingleOrDefault(x => x.Id.Equals(id));

            if (userInDb == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userInDb);
            _context.SaveChanges();

            System.Diagnostics.Debug.WriteLine("Usunieto usera");
            return Ok();
            
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            return Ok(users);
        }


        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            var userInDb = _context.Users.SingleOrDefault(c => c.Id.Equals(id));


            if (userInDb == null)
                return NotFound();

            return Ok(userInDb);
        }


        [HttpPost]
        public IActionResult PostUser([FromBody] User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }

    }
}