using ProvaPub.Application.Exceptions;
using ProvaPub.Application.Interfaces.DataFormatProvider;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Application.Interfaces.Services;
using ProvaPub.Domain;
using ProvaPub.Domain.Service;

namespace ProvaPub.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly PurchasePolicy _purchasePolicy;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CustomerService(
            ICustomerRepository customerRepository, 
            IOrderRepository orderRepository, 
            PurchasePolicy purchasePolicy, 
            IDateTimeProvider dateTimeProvider)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _purchasePolicy = purchasePolicy;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CustomerList> ListCustomersAsync(int page)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            (List<Customer> customer, int totalCount) = await _customerRepository.GetPagedAsync(page, pageSize);

            var hasNext = (page * pageSize) < totalCount;

            return new CustomerList() { HasNext = hasNext, TotalCount = totalCount, Customers = customer };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            ValidateInput(customerId, purchaseValue);

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _customerRepository.GetByIdAsync(customerId) 
                ?? throw new BusinessException("Cliente não encontrado.");

            var hasPurchasedThisMonth = await _orderRepository.HasPurchaseThisMonth(customerId);

            var haveBoughtBefore = await _customerRepository.HaveBoughtBefore(customerId);

            return _purchasePolicy.CanPurchase(
                customer,
                purchaseValue,
                haveBoughtBefore,
                hasPurchasedThisMonth,
                _dateTimeProvider.UtcNow);
        }

        private static void ValidateInput(
        int customerId,
        decimal purchaseValue)
        {
            if (customerId <= 0)
                throw new BusinessException("O valor da propriedade 'CustomerId' não pode ser 0 ou menor que 0.");
            

            if (purchaseValue <= 0)
                throw new BusinessException("O valor da propriedade 'PurchaseValue' não pode ser 0 ou menor que 0.");
        }
    }
}
