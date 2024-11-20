using Microsoft.AspNetCore.Http;
using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.Interfaces;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Store.Services.Infrastructure
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Product GetById(int id)
        {
            return _productRepository.FirstOrDefault(x => x.Id == id);
        }

        public async Task<StoreViewModel> GetProductForStore(int counter, List<int> listOfEmptyStockIds)
        {
            var products = await _productRepository.GetProductForStores(counter, listOfEmptyStockIds);

            var viewModel = new StoreViewModel()
            {
                Products = products
            };

            return viewModel;  
        }

        public async Task<StoreViewModel> GetProductForStoreById(int counter, List<int> listOfEmptyStockIds, int id)
        {
            var products = await _productRepository.GetProductForStoreById(counter, listOfEmptyStockIds, id);

            var viewModel = new StoreViewModel()
            {
                Products = products
            };

            return viewModel;
        }
    }
}
