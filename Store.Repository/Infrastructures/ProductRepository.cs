using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Repositories.Interfaces;
using Store.Services.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Store.Repositories.Infrastructure
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(StoreContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductForStores(int counter, List<int> listOfEmptyStockIds)
        {
            var products = _context.Products
               .Where(x => x.Stock.Any(x => x.IsLastElementOrdered == false))
               .Where(x => x.Stock.Count > 0)
               .Include(u => u.Color)
               .Include(u => u.Sex)
               .Include(u => u.Category);

            if (counter != 0)
            {
                return await FilterProductList(listOfEmptyStockIds, products).ToListAsync();
            }
            else
            {
                return await products.IncludeFilter(x => x.Stock.Where(x => x.IsLastElementOrdered == false)).ToListAsync();
            }
        }


        public async Task<List<Product>> GetProductForStoreById(int counter, List<int> listOfEmptyStockIds, int id)
        {
            var products = _context.Products
               .Where(x => x.Stock.Any(x => x.IsLastElementOrdered == false))
               .Where(x => x.Stock.Count > 0)
               .Where(x=>x.Id == id)
               .Include(u => u.Color)
               .Include(u => u.Sex)
               .Include(u => u.Category);

            if (counter != 0)
            {
                return await FilterProductList(listOfEmptyStockIds, products).ToListAsync();
            }
            else
            {
                return await products.IncludeFilter(x => x.Stock.Where(x => x.IsLastElementOrdered == false)).ToListAsync();
            }
        }

        private IQueryable<Product> FilterProductList(List<int> list, IQueryable<Product> products)
        {
            IQueryable<Product> temp;
            switch (list.Count)
            {
                case 1:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.IsLastElementOrdered == false));
                    break;

                case 2:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1]
                   && x.IsLastElementOrdered == false));
                    break;

                case 3:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                   && x.IsLastElementOrdered == false));
                    break;

                case 4:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3]
                   && x.IsLastElementOrdered == false));
                    break;

                case 5:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4]
                   && x.IsLastElementOrdered == false));
                    break;

                case 6:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5]
                   && x.IsLastElementOrdered == false));
                    break;

                case 7:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6]
                   && x.IsLastElementOrdered == false));
                    break;

                case 8:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7]
                   && x.IsLastElementOrdered == false));
                    break;

                case 9:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8]
                   && x.IsLastElementOrdered == false));
                    break;

                case 10:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]

                   && x.IsLastElementOrdered == false));
                    break;

                case 11:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10]
                   && x.IsLastElementOrdered == false));
                    break;

                case 12:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11]
                   && x.IsLastElementOrdered == false));
                    break;

                case 13:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12]
                   && x.IsLastElementOrdered == false));
                    break;

                case 14:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13]
                   && x.IsLastElementOrdered == false));
                    break;

                case 15:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14]
                   && x.IsLastElementOrdered == false));
                    break;

                case 16:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14] && x.Id != list[15]

                   && x.IsLastElementOrdered == false));
                    break;

                case 17:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14] && x.Id != list[15]
                    && x.Id != list[16]
                   && x.IsLastElementOrdered == false));
                    break;

                case 18:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14] && x.Id != list[15]
                    && x.Id != list[16] && x.Id != list[17]
                   && x.IsLastElementOrdered == false));
                    break;

                case 19:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14] && x.Id != list[15]
                    && x.Id != list[16] && x.Id != list[17] && x.Id != list[18]
                   && x.IsLastElementOrdered == false));
                    break;

                case 20:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.Id != list[0] && x.Id != list[1] && x.Id != list[2]
                    && x.Id != list[3] && x.Id != list[4] && x.Id != list[5] && x.Id != list[6] && x.Id != list[7] && x.Id != list[8] && x.Id != list[9]
                    && x.Id != list[10] && x.Id != list[11] && x.Id != list[12] && x.Id != list[13] && x.Id != list[14] && x.Id != list[15]
                    && x.Id != list[16] && x.Id != list[17] && x.Id != list[18] && x.Id != list[19]
                   && x.IsLastElementOrdered == false));
                    break;

                default:
                    temp = products.IncludeFilter(x => x.Stock.Where(x => x.IsLastElementOrdered == false));
                    break;
            }

            return temp;
        }
    }
}
