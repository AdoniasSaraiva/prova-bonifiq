using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product>, int)> GetPagedAsync(int page, int pageSize);
    }
}
