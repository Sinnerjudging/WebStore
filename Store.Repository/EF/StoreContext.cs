using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Repositories.Entities;
using System;
namespace Store.Services.EF
{
    public class StoreContext : IdentityDbContext<User, Role, int>
    {

        public virtual DbSet<Gender> Genders { get; set; }
        public override DbSet<User> Users { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Sex> Sexes { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }

        public StoreContext()
        {

        }

        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                   .HasOne(x => x.User)
                   .WithMany(i => i.Orders)
                   .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<OrderProduct>()
                   .HasOne(x => x.Product)
                   .WithMany(i => i.OrderProducts)
                   .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<OrderProduct>()
                   .HasOne(x => x.Order)
                   .WithMany(i => i.OrderProducts)
                   .HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<Product>()
                   .HasOne(x => x.Sex)
                   .WithMany(i => i.Products)
                   .HasForeignKey(e => e.SexId)
                   .IsRequired();

            modelBuilder.Entity<Product>()
                   .HasOne(x => x.Category)
                   .WithMany(i => i.Products)
                   .HasForeignKey(e => e.CategoryId)
                   .IsRequired();

            modelBuilder.Entity<Product>()
                   .HasOne(x => x.Color)
                   .WithMany(i => i.Products)
                   .HasForeignKey(e => e.ColorId)
                   .IsRequired();

            modelBuilder.Entity<Gender>()
                .HasData(
                    new Gender
                    {
                        Id = 1,
                        Name = "Male"
                    },
                    new Gender
                    {
                        Id = 2,
                        Name = "Female"
                    },
                    new Gender
                    {
                        Id = 3,
                        Name = "Unknown"
                    }
                );

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "User",
                        NormalizedName = "USER"
                    }
                );

            modelBuilder.Entity<Sex>()
                .HasData(
                    new Gender
                    {
                        Id = 1,
                        Name = "Male"
                    },
                    new Gender
                    {
                        Id = 2,
                        Name = "Female"
                    },
                    new Gender
                    {
                        Id = 3,
                        Name = "Unknown"
                    }
                );

            modelBuilder.Entity<Category>()
                .HasData(
                    new Category
                    {
                        Id = 1,
                        Name = "Koi Kohaku"
                    },
                    new Category
                    {
                        Id = 2,
                        Name = "Koi Sanke"
                    }
                );

            modelBuilder.Entity<Color>()
                .HasData(
                    new Color
                    {
                        Id = 1,
                        Name = "White"
                    },
                    new Color
                    {
                        Id = 2,
                        Name = "Black"
                    },
                    new Color
                    {
                        Id = 3,
                        Name = "Blue"
                    },
                    new Color
                    {
                        Id = 4,
                        Name = "Yellow"
                    },
                    new Color
                    {
                        Id = 5,
                        Name = "Gray"
                    },
                    new Color
                    {
                        Id = 7,
                        Name = "Other"
                    },
                    new Color
                    {
                        Id = 6,
                        Name = "Red"
                    }
                );

            // Dodanie przykładowych produktów do tabeli Products
            modelBuilder.Entity<Product>()
                .HasData(
                    new Product
                    {
                        Id = 1,
                        ColorId = 2,
                        SexId = 3,
                        CategoryId = 1,
                        Name = "Cá koi 01",
                        Price = 399.99m,
                        Description = "Cá koi 01",
                        PhotoPath = "koi01.jpg"
                    },
                    new Product
                    {
                        Id = 2,
                        ColorId = 1,
                        SexId = 1,
                        CategoryId = 1,
                        Name = "Koi red",
                        Price = 199.99m,
                        Description = "A koi red",
                        PhotoPath = "koi02.jpg"
                    },
                    new Product
                    {
                        Id = 3,
                        ColorId = 6,
                        SexId = 2,
                        CategoryId = 1,
                        Name = "Koi 03",
                        Price = 599.99m,
                        Description = "Koi 03",
                        PhotoPath = "koi03.jpg"
                    },
                    new Product
                    {
                        Id = 4,

                        ColorId = 6,
                        SexId = 1,
                        CategoryId = 2,
                        Name = "Koi Yatsushiro",
                        Price = 999.99m,
                        Description = "Koi Yatsushiro",
                        PhotoPath = "koi04.jpg"
                    },
                    new Product
                    {
                        Id = 5,

                        ColorId = 1,
                        SexId = 1,
                        CategoryId = 2,
                        Name = "Buju Banton",
                        Price = 599.99m,
                        Description = "Koi Ai Goromo",
                        PhotoPath = "koi05.jpg"
                    },
                    new Product
                    {
                        Id = 6,
                        ColorId = 1,
                        SexId = 3,
                        CategoryId = 2,
                        Name = "Cá koi Ki Utsuri",
                        Price = 199.99m,
                        Description = "Ki Utsuri is a type of black koi fish with yellow spots all over their body. They are members of the Utsurimono family and are considered one of the rarest koi breeds.",
                        PhotoPath = "koi06.jpg"
                    },
                    new Product
                    {
                        Id = 7,
                        ColorId = 1,
                        SexId = 2,
                        CategoryId = 1,
                        Name = "Koi Ki Matsuba",
                        Price = 299.99m,
                        Description = "The Matsuba family of koi are known for their gray or metallic body with a black “mesh” pattern on top!",
                        PhotoPath = "koi07.jpg"
                    }

                );

            modelBuilder.Entity<Stock>()
                .HasData(
                    new Stock
                    {
                        ProductId = 1,
                        Id = 1,
                        IsLastElementOrdered = false,
                        Name = "10",
                        Qty = 3
                    },
                    new Stock
                    {
                        ProductId = 1,
                        Id = 2,
                        IsLastElementOrdered = false,
                        Name = "11",
                        Qty = 2
                    },
                    new Stock
                    {
                        ProductId = 1,
                        Id = 3,
                        IsLastElementOrdered = false,
                        Name = "12",
                        Qty = 1
                    },
                    new Stock
                    {
                        ProductId = 4,
                        Id = 4,
                        IsLastElementOrdered = false,
                        Name = "S",
                        Qty = 2
                    },
                    new Stock
                    {
                        ProductId = 4,
                        Id = 5,
                        IsLastElementOrdered = false,
                        Name = "L",
                        Qty = 2
                    },
                    new Stock
                    {
                        ProductId = 4,
                        Id = 6,
                        IsLastElementOrdered = false,
                        Name = "XL",
                        Qty = 1
                    },
                     new Stock
                     {
                         ProductId = 7,
                         Id = 7,
                         IsLastElementOrdered = false,
                         Name = "9",
                         Qty = 1
                     },
                     new Stock
                     {
                         ProductId = 2,
                         Id = 8,
                         IsLastElementOrdered = false,
                         Name = "11",
                         Qty = 3
                     }
                );


            User adminUser = new User
            {
                UserName = "admin",
                Email = "admin@store.com",
                NormalizedEmail = "admin@store.com".ToUpper(),
                NormalizedUserName = "admin".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false,
                City = "admin",
                FirstName = "admin",
                LastName = "admin",
                GenderId = 3,
                Address1 = "admin",
                PostCode = "51-627",
                Id = 1,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User user = new User
            {
                UserName = "user",
                Email = "user@store.com",
                NormalizedEmail = "user@store.com".ToUpper(),
                NormalizedUserName = "user".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "987654321",
                PhoneNumberConfirmed = false,
                City = "user",
                FirstName = "user",
                LastName = "user",
                GenderId = 1,
                Address1 = "user",
                PostCode = "51-627",
                Id = 2,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            PasswordHasher<User> ph = new PasswordHasher<User>();

            adminUser.PasswordHash = ph.HashPassword(adminUser, "admin");
            user.PasswordHash = ph.HashPassword(user, "user");

            var adminrole = new IdentityUserRole<int>
            { RoleId = 1, UserId = 1 };
            var userrole = new IdentityUserRole<int>
            { RoleId = 2, UserId = 2 };

            modelBuilder.Entity<User>().HasData(
                adminUser,
                user
            );

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                adminrole,
                userrole
            );

            modelBuilder.Entity<AppUser>()
               .HasData(
                   new AppUser
                   {
                       Id = 1,
                       UserId = 1
                   },
                   new AppUser
                   {
                       Id = 2,
                       UserId = 2
                   }
             );
        }
    }
}
