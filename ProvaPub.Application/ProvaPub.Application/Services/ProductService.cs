using ProvaPub.Application.Interfaces.Services;
using ProvaPub.Domain;
using ProvaPub.Application.Interfaces.Repositories;

namespace ProvaPub.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
             _productRepository = productRepository;
        }

        public async Task<ProductList> ListProductsAsync(int page)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            (List<Product> products, int totalCount) = await _productRepository.GetPagedAsync(page, pageSize);

            var hasNext = (page * pageSize) < totalCount;

            return new ProductList() { HasNext = hasNext, TotalCount = totalCount, Products = products };
        }
    }
}
