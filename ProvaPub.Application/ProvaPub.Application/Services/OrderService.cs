using ProvaPub.Application.Exceptions;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Application.Interfaces.Services;
using ProvaPub.Application.Interfaces.Strategy;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
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
                CustomerId = customerId,
                PaymentMethod = strategy.PaymentMethod,
            });

            orderCreated.OrderDate = orderCreated.OrderDate.AddHours(-3);

            return orderCreated;
        }
    }
}
