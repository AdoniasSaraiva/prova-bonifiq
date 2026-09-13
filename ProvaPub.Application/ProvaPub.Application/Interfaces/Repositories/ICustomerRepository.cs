using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<(List<Customer>, int)> GetPagedAsync(int page, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> HaveBoughtBefore(int customerId);
    }
}
