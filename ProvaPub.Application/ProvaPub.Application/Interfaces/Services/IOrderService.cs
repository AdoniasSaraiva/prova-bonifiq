using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Services
{
    public interface IOrderService
    {
        public Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId);
    }
}
