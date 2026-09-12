using ProvaPub.Models;

namespace ProvaPub.Repository.Inteface
{
    public interface IOrderRepository
    {
        Task<int> ManyTimesCustomerPurchaseInMonth(int CustomerId, DateTime baseDate);

        Task<Order> InsertOrder(Order order);
    }
}
