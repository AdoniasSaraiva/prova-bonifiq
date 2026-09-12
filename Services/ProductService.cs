using ProvaPub.Models;
using ProvaPub.Repository.Inteface;
using ProvaPub.Services.Interface;

namespace ProvaPub.Services
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

            return new ProductList() { HasNext = hasNext, TotalCount = products.Count, Products = products };
        }
    }
}
