using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Infra.Data.Context;

namespace ProvaPub.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TestDbContext _context;
        public ProductRepository(TestDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Product>, int)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = _context.Products.Count();

            return (await _context.Products
                                    .OrderBy(p => p.Id)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(), totalCount);
        }
    }
}
