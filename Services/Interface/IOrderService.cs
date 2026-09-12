using ProvaPub.Models;

namespace ProvaPub.Services.Interface
{
    public interface IOrderService
    {
        public Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId);
    }
}
