namespace ProvaPub.Patterns.Strategy.PaymentStrategyMethod
{
    public class PixPaymentStrategy : IPaymentStrategy
    {
        public string PaymentMethod => "pix";

        public async Task ProcessPaymentAsync(decimal paymentValue, int customerId)
        {
            // Lógica de pagamento via Pix

        }
    }
}
