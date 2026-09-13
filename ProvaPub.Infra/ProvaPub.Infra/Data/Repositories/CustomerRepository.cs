using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Infra.Data.Context;

namespace ProvaPub.Infra.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TestDbContext _context;
        public CustomerRepository(TestDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer;
        }

        public async Task<(List<Customer>, int)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = _context.Customers.Count();

            return (await _context.Customers
                                    .OrderBy(p => p.Id)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(), totalCount);
        }

        public async Task<bool> HaveBoughtBefore(int customerId) 
        {
            var haveBoughtBefore = await _context.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            return haveBoughtBefore > 0;
        }
    }
}
