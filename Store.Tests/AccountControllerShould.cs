using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Store.Controllers;
using Store.Models;
using Store.Models.DTO;
using Store.Tests.MockData;
using Store.Tests.MockData.MockIdentity;
using Store.ViewModels;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace Store.Tests
{
    public class AccountControllerShould : StoreMockContext
    {
        private Mock<FakeSignInManager> _mockSignInManager;
        private Mock<FakeUserManager> _mockUserManager;
        private Mock<IWebHostEnvironment> _mockWebHostEnviroment;
        private IMapper _mapper;
        private DomainProfile domainProfile;
        private MapperConfiguration configuration;


        private AccountController _sut;


        public AccountControllerShould()
        {
            _mockUserManager = new FakeUserManagerBuilder().Build();
            _mockSignInManager = new FakeSignInManagerBuilder().Build();
            _mockWebHostEnviroment = new Mock<IWebHostEnvironment>();

            //mapper configuration
            domainProfile = new DomainProfile();
            configuration = new MapperConfiguration(x => x.AddProfile(domainProfile));
            _mapper = new Mapper(configuration);

            _sut = new AccountController(_context, _mockUserManager.Object, _mockSignInManager.Object, _mockWebHostEnviroment.Object, _mapper);
        }



        [Fact]
        public void ReturnViewForRegister()
        {
            //Act
            IActionResult result = _sut.Register();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ReturnViewForLogin()
        {
            //Act
            IActionResult result = _sut.Login();

            //Assert
            Assert.IsType<ViewResult>(result);
        }



        [Fact]
        public async void RedirectUserToLocalPageAfterSuccessfulLoginIfHeWasLoggingFromLocalPage()
        {
            //Arrange
            _mockSignInManager = new FakeSignInManagerBuilder()
                .With(x => x.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success))
                .Build();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.IsLocalUrl(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            _sut = new AccountController(_context, _mockUserManager.Object, _mockSignInManager.Object, _mockWebHostEnviroment.Object, _mapper);

            //Act
            _sut.Url = mockUrlHelper.Object;
            var result = await _sut.Login(new LoginViewModel(), "testPath");

            //Assert
            Assert.IsType<RedirectResult>(result);
        }


        [Fact]
        public async void RedirectUserToHomePageAfterSuccessfulLoginIfHeWasLoggingNotFromLocalPage()
        {
            //Arrange
            _mockSignInManager = new FakeSignInManagerBuilder()
                .With(x => x.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>()))
                    .ReturnsAsync(SignInResult.Success))
                .Build();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();
            _sut = new AccountController(_context, _mockUserManager.Object, _mockSignInManager.Object, _mockWebHostEnviroment.Object, _mapper);


            //Act
            _sut.Url = mockUrlHelper.Object;
            var result = await _sut.Login(new LoginViewModel(), "testPath");

            //Assert
            var actionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(actionResult.ActionName, $"Index");
            Assert.Equal(actionResult.ControllerName, $"Home");

        }

        [Fact]
        public async void RedirectUserToLoginViewIfModelIsInvalidWithData()
        {

            //Act
            _sut.ModelState.AddModelError("x", "test error");
            var result = await _sut.Login(new LoginViewModel(), "testPath");

            //Assert
            var actionResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LoginViewModel>(actionResult.Model);
            Assert.Equal(actionResult.ViewName, $"Login");
        }


        //register
        [Fact]
        public async void RedirectUserToHomePageAfterSuccessfulRegistration()
        {
            //Arrange
            _mockSignInManager = new FakeSignInManagerBuilder()
                .With(x => x.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>()))
                    .ReturnsAsync(SignInResult.Success))
                .Build();

            _mockUserManager = new FakeUserManagerBuilder()
                .With(x => x.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success)).Build();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.IsLocalUrl(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            _sut = new AccountController(_context, _mockUserManager.Object, _mockSignInManager.Object, _mockWebHostEnviroment.Object, _mapper);


            //Act
            _sut.Url = mockUrlHelper.Object;
            var result = await _sut.Register(new RegisterViewModel());

            //Assert
            var actionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(actionResult.ActionName, $"Index");
            Assert.Equal(actionResult.ControllerName, $"Home");

        }

        [Fact]
        public async void RedirectUserToRegisterViewIfModelIsInvalidWithData()
        {

            //Act
            _sut.ModelState.AddModelError("x", "test error");
            var testViewModel = new RegisterViewModel
            { FirstName = "Test", LastName = "Test2", UserName = "Test3", GenderId = 1 };
            var result = await _sut.Register(testViewModel);

            //Assert
            var actionResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RegisterViewModel>(actionResult.Model);

            Assert.Equal(actionResult.ViewName, $"Register");
            Assert.Equal(model.FirstName, testViewModel.FirstName);
            Assert.Equal(model.LastName, testViewModel.LastName);
            Assert.Equal(model.UserName, testViewModel.UserName);
            Assert.Equal(model.GenderId, testViewModel.GenderId);

        }


        //logout
        [Fact]
        public async void RedirectUserToIndexViewWhenLogout()
        {

            //Act
            var result = await _sut.Logout();

            //Assert
            var actionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(actionResult.ActionName, $"Index");
        }



















    }
}
