using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Store.Controllers;
using Store.Models;
using Store.Models.DTO;
using Store.Tests.MockData;
using Store.Tests.MockData.MockIdentity;
using Store.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Store.Tests
{
    public class AdministrationControllerShould : StoreMockContext
    {

        private Mock<FakeRoleManager> _mockRoleManager;
        private Mock<FakeUserManager> _mockUserManager;
        private Mock<FakeSignInManager> _mockSignInManager;
        private Mock<IWebHostEnvironment> _mockWebHostEnviroment;
        private IMapper _mapper;
        private DomainProfile domainProfile;
        private MapperConfiguration configuration;


        private AdministrationController _sut;

        public AdministrationControllerShould()
        {
            _mockUserManager = new FakeUserManagerBuilder().Build();
            _mockWebHostEnviroment = new Mock<IWebHostEnvironment>();
            _mockRoleManager = new FakeRoleManagerBuilder().Build();
            _mockSignInManager = new FakeSignInManagerBuilder().Build();

            //mapper configuration
            domainProfile = new DomainProfile();
            configuration = new MapperConfiguration(x => x.AddProfile(domainProfile));
            _mapper = new Mapper(configuration);
            _sut = new AdministrationController(_mockRoleManager.Object, _mockUserManager.Object, _context, _mockWebHostEnviroment.Object, _mapper, _mockSignInManager.Object);


        }

        //userform
        [Fact]
        public async void ReturnViewForUserFormWithData()
        {
            //in fake database created user with id = 1
            var result = await _sut.EditUser(1);
            var user = await _context.Users.SingleAsync(x => x.Id.Equals(1));


            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserFormViewModel>(view.Model);
            Assert.Equal(model.FirstName, user.FirstName);
            Assert.Equal(model.LastName, user.LastName);
            Assert.Equal(model.GenderId, user.GenderId);

        }

        [Fact]
        public void ReturnViewForIndex()
        {
            //Act
            IActionResult result = _sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task ReturnViewForUsers()
        {
            //Act
            IActionResult result = await _sut.Users();

            //Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void ReturnViewForCreateRole()
        {
            //Act
            IActionResult result =  _sut.CreateRole();

            //Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void ReturnViewForRoles()
        {
            //Act
            IActionResult result = _sut.Roles();

            //Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task ReturnNotFoundWhenNotSuchRole()
        {

            //Act
            IActionResult result = await _sut.EditRole(9999);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }



        //AddProduct Tests
        [Fact]
        public void ReturnViewForAddProduct()
        {
            //Act
            IActionResult result = _sut.AddProduct();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWithDataWhenInvalidModelStateInProductForm()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var productViewModel = new ProductFormViewModel
            {
                Name = "Test",
                ColorId = 2,
                Description = "test",
                SexId = 1,
                Price = "20.99",
                CategoryId=1,
                PhotoPath="XDDD"
            };

            //Act
            IActionResult result = await _sut.SaveProduct(productViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductFormViewModel>(viewResult.Model);
            Assert.Equal(productViewModel.Name, model.Name);
            //Assert.Equal(productViewModel.BrandId, model.BrandId);
            Assert.Equal(productViewModel.ColorId, model.ColorId);
            Assert.Equal(productViewModel.Description, model.Description);
            Assert.Equal(productViewModel.SexId, model.SexId);
            Assert.Equal(productViewModel.SexId, model.SexId);
            Assert.Equal(productViewModel.CategoryId, model.CategoryId);
        }

        [Fact]
        public async Task NotSaveProductWhenModelError()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var productViewModel = new ProductFormViewModel { Name = "NoSaveTest", Price = "20.00", ColorId = 1, SexId = 1, CategoryId = 1, Description = "NoSaveTest",PhotoPath="XD" };

            //Act
            await _sut.SaveProduct(productViewModel);
            var result = _context.Products.FirstOrDefault(x => x.Name.Equals("NoSaveTest"));

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task SaveProductWhenValidModel()
        {
            //Arrange
            var productViewModel = new ProductFormViewModel() { Name = "SaveTest", Price = "2000",  ColorId = 1, SexId = 1, CategoryId = 1, Description = "SaveTest", PhotoPath = "XD" };

            //Act
            await _sut.SaveProduct(productViewModel);
            var savedType = _context.Products.FirstOrDefault(x => x.Name.Equals("SaveTest"));


            //Assert
            Assert.NotNull(savedType);
            Assert.Equal(productViewModel.Name, savedType.Name);
        }

        [Fact]
        public async Task RedirectToIndexWhenValidModel()
        {
            //Arrange
            var productViewModel = new ProductFormViewModel() { Name = "SaveTest", Price = "2000", ColorId = 1, SexId = 1, CategoryId = 1, Description = "SaveTest" };

            //Act
            var result = await _sut.SaveProduct(productViewModel);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            //Assert
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        ///////////////////////////////////////////
        //////////////////////////////////////////////
        ///////////////////////
        ///

        //AddType Tests
        [Fact]
        public void ReturnViewForAddCategory()
        {
            //Act
            IActionResult result = _sut.AddCategory();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWithDataWhenInvalidModelStateInCategoryForm()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var category = new Category
            {
                Name = "Test"
            };

            //Act
            IActionResult result = await _sut.SaveCategory(category);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Category>(viewResult.Model);
            Assert.Equal(category.Name, model.Name);

        }

        [Fact]
        public async Task NotSaveCategoryWhenModelError()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var category = new Category
            {
                Name = "NoSaveTest"
            };

            //Act
            await _sut.SaveCategory(category);
            var result = _context.Categories.FirstOrDefault(x => x.Name.Equals("NoSaveTest"));

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task SaveCategoryWhenValidModel()
        {
            //Arrange
            var category = new Category
            {
                Name = "TestSave"
            };

            //Act
            await _sut.SaveCategory(category);
            var savedCategory = _context.Categories.FirstOrDefault(x => x.Name.Equals("TestSave"));


            //Assert
            Assert.NotNull(savedCategory);
            Assert.Equal(category.Name, savedCategory.Name);
        }

        [Fact]
        public async Task RedirectToCategoriesWhenValidModel()
        {
            //Arrange
            var category = new Category
            {
                Name = "Test name"
            };

            //Act
            var result = await _sut.SaveCategory(category);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            //Assert
            Assert.Equal("Category", redirectToActionResult.ActionName);
        }

        //ColorFormTests

        [Fact]
        public void ReturnViewForColor()
        {
            //Act
            IActionResult result = _sut.AddColor();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWithDataWhenInvalidModelStateInColorForm()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var color = new Color
            {
                Name = "Test"
            };

            //Act
            IActionResult result = await _sut.SaveColor(color);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Color>(viewResult.Model);
            Assert.Equal(color.Name, model.Name);
        }

        ////////////////
        /////////////////
        //////////////////////
        ///////////////////////

        [Fact]
        public async Task NotSaveColorWhenModelErrorAsync()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var color = new Color
            {
                Name = "NoSaveTest"
            };

            //Act
            await _sut.SaveColor(color);
            var result = _context.Colors.FirstOrDefault(x => x.Name.Equals("NoSaveTest"));

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task SaveColorWhenValidModelAsync()
        {
            //Arrange
            var color = new Color
            {
                Name = "TestSave"
            };

            //Act
            await _sut.SaveColor(color);
            var savedColor = _context.Colors.FirstOrDefault(x => x.Name.Equals("TestSave"));


            //Assert
            Assert.NotNull(savedColor);
            Assert.Equal(color.Name, savedColor.Name);
        }

        [Fact]
        public async Task RedirectToColorWhenValidModelAsync()
        {
            //Arrange
            var color = new Color
            {
                Name = "Test name"
            };

            //Act
            var result = await _sut.SaveColor(color);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            //Assert
            Assert.Equal("Color", redirectToActionResult.ActionName);
        }

        //BrandFormTests
        [Fact]
        public void ReturnViewForBrand()
        {
            //Act
            IActionResult result = _sut.AddBrand();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWithDataWhenInvalidModelStateInBrandFormAsync()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var brand = new Brand
            {
                Name = "Test",
                Description = "Test description",
                PhotoPath="Test photo path"
            };

            //Act
            IActionResult result = await _sut.SaveBrand(brand);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Brand>(viewResult.Model);
            Assert.Equal(brand.Name, model.Name);
        }

        [Fact]
        public async Task NotSaveBrandWhenModelErrorAsync()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var brand = new Brand
            {
                Name = "NoSaveTest",
                Description = "No save test description"
            };

            //Act
            await _sut.SaveBrand(brand);
            var result = _context.Brands.FirstOrDefault(x => x.Name.Equals("NoSaveTest"));

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task SaveBrandWhenValidModelAsync()
        {
            //Arrange
            var brand = new Brand
            {
                Name = "TestSave",
                Description = "Test description save",
                PhotoPath="Test photo path"
            };

            //Act
            await _sut.SaveBrand(brand);
            var savedBrand = _context.Brands.FirstOrDefault(x => x.Name.Equals("TestSave"));


            //Assert
            Assert.NotNull(savedBrand);
            Assert.Equal(brand.Name, savedBrand.Name);
        }

        [Fact]
        public async Task RedirectToBrandWhenValidModelAsync()
        {
            //Arrange
            var brand = new Brand
            {
                Name = "Test name",
                Description = "Test description",
                PhotoPath="Test path"
            };

            //Act
            var result = await _sut.SaveBrand(brand);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            //Assert
            Assert.Equal("Brand", redirectToActionResult.ActionName);
        }


        //SizeFormTests
        [Fact]
        public void ReturnViewForStock()
        {
            var productInMockDbId = 100;
            //Act
            IActionResult result = _sut.Stock(productInMockDbId);

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWithDataWhenInvalidModelStateInStockFormAsync()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var stockViewModel = new StockViewModel
            {
                Name = "Test",
                ProductId=1,
                Qty=1,
            };

            //Act
            IActionResult result = await _sut.Stock(stockViewModel);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StockViewModel>(viewResult.Model);
            Assert.Equal(stockViewModel.Name, model.Name);
            Assert.Equal(stockViewModel.Qty, model.Qty);
            Assert.Equal(stockViewModel.ProductId, model.ProductId);

        }

        [Fact]
        public async Task NotSaveStockWhenModelErrorAsync()
        {
            //Arrange
            _sut.ModelState.AddModelError("x", "Test Error");

            var stockViewModel = new StockViewModel
            {
                Name = "NoSaveTest",
                ProductId = 1,
                Qty = 1

            };

            //Act
            await _sut.Stock(stockViewModel);
            var result = _context.Stock.FirstOrDefault(x => x.Name.Equals("NoSaveTest"));

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task SaveStockWhenValidModelAsync()
        {
            //Arrange
            var stockViewModel = new StockViewModel
            {
                Name = "TestSave",
                ProductId = 1,
                Qty = 1
            };

            //Act
            await _sut.Stock(stockViewModel);
            var savedType = _context.Stock.FirstOrDefault(x => x.Name.Equals("TestSave"));


            //Assert
            Assert.NotNull(savedType);
            Assert.Equal(stockViewModel.Name, savedType.Name);
        }


    }
}
