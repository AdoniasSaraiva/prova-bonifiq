namespace ProvaPub.Patterns.Strategy.PaymentStrategyMethod
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
