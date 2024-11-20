using AutoMapper;
using Store.ViewModels;

namespace Store.Models.DTO
{
    public class DomainProfile : Profile
    {

        public DomainProfile()
        {
            CreateMap<User, User>();
            CreateMap<UserFormViewModel, User>();
            CreateMap<User,UserFormViewModel>();
            CreateMap<User, RegisterViewModel>();
            CreateMap<RegisterViewModel, User>();
            CreateMap<Product, ProductFormViewModel>();
            CreateMap<ProductFormViewModel, Product>();
            CreateMap<StockViewModel, Stock>();
            CreateMap<Order, Order>();
           


        }
    }
}
