using ProvaPub.Models;

namespace ProvaPub.Repository.Inteface
{
    public interface ICustomerRepository
    {
        Task<(List<Customer>, int)> GetPagedAsync(int page, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> HaveBoughtBefore(int customerId);
    }
}
