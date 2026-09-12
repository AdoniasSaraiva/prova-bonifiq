namespace ProvaPub.Patterns.Strategy.PaymentStrategyMethod
{
    public class PaypalPaymentStrategy : IPaymentStrategy
    {
        public string PaymentMethod => "paypal";

        public async Task ProcessPaymentAsync(decimal paymentValue, int customerId)
        {
            // Lógica de pagamento via PayPal
        }
    }
}
