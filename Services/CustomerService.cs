using ProvaPub.Exception;
using ProvaPub.Models;
using ProvaPub.Repository.Inteface;
using ProvaPub.Services.Interface;

namespace ProvaPub.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CustomerService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
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
            if (customerId <= 0) throw new BusinessException("O valor da propriedade 'CustomerId' não poder ser 0 ou menor que 0.");

            if (purchaseValue <= 0) throw new BusinessException("O valor da propriedade 'PurchaseValue' não poder ser 0 ou menor que 0.");

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _customerRepository.GetByIdAsync(customerId) ??
                throw new BusinessException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = DateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _orderRepository.ManyTimesCustomerPurchaseInMonth(customerId, baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _customerRepository.HaveBoughtBefore(customerId);
            if (!haveBoughtBefore && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            if (DateTime.UtcNow.Hour < 8 || DateTime.UtcNow.Hour > 18 || DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                return false;


            return true;
        }
    }
}
