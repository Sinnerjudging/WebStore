using Microsoft.EntityFrameworkCore;
using Store.Models;
using System;
using System.Collections.Generic;

namespace Store.Tests.MockData
{
    public class StoreMockContext : IDisposable
    {
        protected readonly StoreContext _context;

        public StoreMockContext()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StoreContext(options);

            _context.Database.EnsureCreated();
            Seed();
        }

        public void Seed()
        {
            var products = new List<Product>
            {
                new Product
                {
                     Name = "Some Kicks", ColorId = 1, Description = "Some description about kicks",
                    Id = 100, PhotoPath = "Path to kicks image", CategoryId = 1, Price = 100.50M, SexId = 1
                },
                new Product
                {
                    Name = "Some Kicks2", ColorId = 2, Description = "Some description about kicks2",
                    Id = 101, PhotoPath = "Path to kicks image2", CategoryId = 2, Price = 200.00M, SexId = 2
                },

            };

            var users = new List<User>()
            {
                new User {UserName = "TestUserName", FirstName = "Test", LastName = "Name", Email = "test@gmail.com", GenderId = 1, Id=100}
            };

            var roles = new List<Role>()
            {
                new Role() {Id = 100, Name = "Test Role",NormalizedName = "TEST ROLE"}
            };


            foreach (var product in products)
            {
                _context.Products.Add(product);
            }

            foreach (var user in users)
            {
                _context.Users.Add(user);
            }


            foreach (var role in roles)
            {
                _context.Roles.Add(role);
            }


            _context.SaveChanges();
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();

            _context.Dispose();
        }

    }
}
