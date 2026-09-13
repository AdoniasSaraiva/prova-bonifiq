using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        public Task<CustomerList> ListCustomersAsync(int page);

        public Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
