using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Services
{
    public interface IProductService
    {
        public Task<ProductList> ListProductsAsync(int page);
    }
}
