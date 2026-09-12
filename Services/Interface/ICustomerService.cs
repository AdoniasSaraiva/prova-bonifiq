using ProvaPub.Models;

namespace ProvaPub.Services.Interface
{
    public interface ICustomerService
    {
        public Task<CustomerList> ListCustomersAsync(int page);

        public Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
