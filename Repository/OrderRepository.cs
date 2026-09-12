using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository.Inteface;

namespace ProvaPub.Repository
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
            return (await _context.Orders.AddAsync(order)).Entity;
        }
    }
}
