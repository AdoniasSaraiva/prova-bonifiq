using ProvaPub.Models;

namespace ProvaPub.Services.Interface
{
    public interface IProductService
    {
        public Task<ProductList> ListProductsAsync(int page);
    }
}
