using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<int> ManyTimesCustomerPurchaseInMonth(int CustomerId, DateTime baseDate);

        Task<Order> InsertOrder(Order order);

        Task<bool> HasPurchaseThisMonth(int customerId);
    }
}
