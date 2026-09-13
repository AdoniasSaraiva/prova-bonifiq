using ProvaPub.Application.Interfaces.Strategy;

namespace ProvaPub.Infra.PaymentMethod.PaymentStrategyMethod
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        public string PaymentMethod => "creditcard";

        public async Task ProcessPaymentAsync(decimal paymentValue, int customerId)
        {
            // Lógica de pagamento via Cartão
        }
    }
}
