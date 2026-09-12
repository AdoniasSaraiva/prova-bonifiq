using ProvaPub.Exception;
using ProvaPub.Models;
using ProvaPub.Patterns.Strategy;
using ProvaPub.Repository.Inteface;
using ProvaPub.Services.Interface;

namespace ProvaPub.Services
{
    public class OrderService : IOrderService
    {
        private readonly IEnumerable<IPaymentStrategy> _paymentStrategies;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IEnumerable<IPaymentStrategy> paymentStrategies, IOrderRepository orderRepository)
        {
            _paymentStrategies = paymentStrategies;
            _orderRepository = orderRepository;
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
        {
            var strategy = _paymentStrategies.FirstOrDefault(s =>
                s.PaymentMethod.Equals(paymentMethod, StringComparison.OrdinalIgnoreCase))
                ?? throw new BusinessException($"Meio de pagamento '{paymentMethod}' não é suportado.");

            await strategy.ProcessPaymentAsync(paymentValue, customerId);

            var orderCreated = await _orderRepository.InsertOrder(new Order() //Retorna o pedido para o controller
            {
                Value = paymentValue,
                CustomerId = customerId
            });

            orderCreated.OrderDate = orderCreated.OrderDate.AddHours(-3);

            return orderCreated;
        }
    }
}
