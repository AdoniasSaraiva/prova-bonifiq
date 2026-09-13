namespace ProvaPub.Application.Interfaces.Strategy
{
    public interface IPaymentStrategy
    {
        string PaymentMethod { get; }

        Task ProcessPaymentAsync(decimal paymentValue, int customerId);
    }
}
