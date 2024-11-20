using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Store.Models;
using Store.Repositories.Infrastructure;
using Store.Repositories.Infrastructures;
using Store.Repositories.Interfaces;
using Store.Services.EF;
using Store.Services.Infrastructues;
using Store.Services.Infrastructure;
using Store.Services.Interfaces;
using Stripe;
using System;

namespace Store
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Dodanie generatora swaggera, definicja dokumentu Swaggera
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Store API",
                    Version = "v1",
                    Description = "Store API v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Jakub Pietrus, Jakub Zatoñ, Hubert Górski",
                        Url = new Uri("https://github.com/pieetrus/.Net-i-Java/tree/master/Sklep-Projekt")
                    },
                });
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<StoreContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderProductRepository, OrderProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGenderRepository, GenderRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IProductService, Store.Services.Infrastructure.ProductService>();


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });


            services.AddDbContext<StoreContext>(opt =>opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("Store")));

            services.AddControllersWithViews();

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddSession();

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));



        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API v1");
            });

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];


            app.UseDeveloperExceptionPage();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
