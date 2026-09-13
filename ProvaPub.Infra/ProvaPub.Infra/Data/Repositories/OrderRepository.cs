using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Infra.Data.Context;

namespace ProvaPub.Infra.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TestDbContext _context;
        public OrderRepository(TestDbContext context) 
        {
            _context = context;
        }

        public async Task<int> ManyTimesCustomerPurchaseInMonth(int CustomerId, DateTime baseDate)
        {
            return await _context.Orders.CountAsync(s => s.CustomerId == CustomerId && s.OrderDate >= baseDate);
        }

        public async Task<Order> InsertOrder(Order order)
        {
            //Insere pedido no banco de dados
            var orderCreated = (await _context.Orders.AddAsync(order)).Entity;
            await _context.SaveChangesAsync();

            return orderCreated;
        }

        public async Task<bool> HasPurchaseThisMonth(int customerId)
        {
            var now = DateTime.UtcNow;

            var firstDayOfMonth =
                new DateTime(now.Year, now.Month, 1);

            var firstDayOfNextMonth =
                firstDayOfMonth.AddMonths(1);

            return await _context.Orders
                .AnyAsync(order =>
                    order.CustomerId == customerId &&
                    order.OrderDate >= firstDayOfMonth &&
                    order.OrderDate < firstDayOfNextMonth);
        }
    }
}
