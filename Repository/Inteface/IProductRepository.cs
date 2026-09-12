using ProvaPub.Models;

namespace ProvaPub.Repository.Inteface
{
    public interface IProductRepository
    {
        Task<(List<Product>, int)> GetPagedAsync(int page, int pageSize);
    }
}
