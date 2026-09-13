using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Infra.Data.Context;
using ProvaPub.Infra.Data.Repositories;

namespace ProvaPub.Tests.Repositories
{
    [TestFixture]
    public class OrderRepositoryTests : IDisposable
    {
        private TestDbContext _context;
        private OrderRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Arrange - Criar contexto em memória para cada teste
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            _repository = new OrderRepository(_context);

            // Seed de dados iniciais
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Cliente A", Orders = new List<Order>() },
                new Customer { Id = 2, Name = "Cliente B", Orders = new List<Order>() }
            };

            _context.Customers.AddRange(customers);
            _context.SaveChanges();

            var orders = new List<Order>
            {
                new Order { Id = 1, Value = 100, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" },
                new Order { Id = 2, Value = 200, CustomerId = 1, OrderDate = DateTime.UtcNow.AddDays(-5), PaymentMethod = "CreditCard" },
                new Order { Id = 3, Value = 150, CustomerId = 2, OrderDate = DateTime.UtcNow.AddMonths(-2), PaymentMethod = "Paypal" }
            };

            _context.Orders.AddRange(orders);
            _context.SaveChanges();
        }

        #region ManyTimesCustomerPurchaseInMonth Tests

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_WithPurchasesInMonth_ShouldReturnCorrectCount()
        {
            // Arrange
            int customerId = 1;
            var baseDate = DateTime.UtcNow.AddMonths(-1);

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(customerId, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_WithoutPurchasesInPeriod_ShouldReturnZero()
        {
            // Arrange
            int customerId = 2;
            var baseDate = DateTime.UtcNow.AddMonths(-1);

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(customerId, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_NonExistentCustomer_ShouldReturnZero()
        {
            // Arrange
            int nonExistentCustomerId = 999;
            var baseDate = DateTime.UtcNow.AddMonths(-1);

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(nonExistentCustomerId, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_BoundaryDate_ShouldIncludeExactDate()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var exactMonthAgo = now.AddMonths(-1);

            var customer = new Customer { Id = 10, Name = "Test Customer", Orders = new List<Order>() };
            _context.Customers.Add(customer);

            var order = new Order { Id = 100, Value = 100, CustomerId = 10, OrderDate = exactMonthAgo, PaymentMethod = "Pix" };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(10, exactMonthAgo);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_BeforeBoundaryDate_ShouldNotBeIncluded()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var beforeMonthAgo = now.AddMonths(-1).AddSeconds(-1);

            var customer = new Customer { Id = 11, Name = "Test Customer 2", Orders = new List<Order>() };
            _context.Customers.Add(customer);

            var order = new Order { Id = 101, Value = 100, CustomerId = 11, OrderDate = beforeMonthAgo, PaymentMethod = "CreditCard" };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var baseDate = now.AddMonths(-1);

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(11, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_MultipleOrdersInPeriod_ShouldCountAll()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var baseDate = now.AddMonths(-1);

            var customer = new Customer { Id = 12, Name = "Multi Order Customer", Orders = new List<Order>() };
            _context.Customers.Add(customer);

            var orders = new List<Order>
            {
                new Order { Id = 102, Value = 100, CustomerId = 12, OrderDate = now, PaymentMethod = "Pix" },
                new Order { Id = 103, Value = 100, CustomerId = 12, OrderDate = now.AddDays(-5), PaymentMethod = "CreditCard" },
                new Order { Id = 104, Value = 100, CustomerId = 12, OrderDate = now.AddDays(-10), PaymentMethod = "Paypal" }
            };

            _context.Orders.AddRange(orders);
            _context.SaveChanges();

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(12, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task ManyTimesCustomerPurchaseInMonth_ZeroCustomerId_ShouldReturnZero()
        {
            // Arrange
            int zeroId = 0;
            var baseDate = DateTime.UtcNow.AddMonths(-1);

            // Act
            var result = await _repository.ManyTimesCustomerPurchaseInMonth(zeroId, baseDate);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region InsertOrder Tests

        [Test]
        public async Task InsertOrder_WithValidOrder_ShouldInsertAndReturn()
        {
            // Arrange
            var newOrder = new Order { Value = 300, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" };

            // Act
            var result = await _repository.InsertOrder(newOrder);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(300));
            Assert.That(result.CustomerId, Is.EqualTo(1));
        }

        [Test]
        public async Task InsertOrder_ShouldPersistToDatabase()
        {
            // Arrange
            var newOrder = new Order { Value = 250, CustomerId = 2, OrderDate = DateTime.UtcNow, PaymentMethod = "CreditCard" };

            // Act
            var result = await _repository.InsertOrder(newOrder);
            var retrievedOrder = await _context.Orders.FindAsync(result.Id);

            // Assert
            Assert.IsNotNull(retrievedOrder);
            Assert.That(retrievedOrder.Value, Is.EqualTo(250));
            Assert.That(retrievedOrder.CustomerId, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertOrder_MultipleOrders_ShouldInsertAllWithDifferentIds()
        {
            // Arrange
            var order1 = new Order { Value = 100, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" };
            var order2 = new Order { Value = 200, CustomerId = 2, OrderDate = DateTime.UtcNow, PaymentMethod = "Paypal" };

            // Act
            var result1 = await _repository.InsertOrder(order1);
            var result2 = await _repository.InsertOrder(order2);

            // Assert
            Assert.That(result1.Id, Is.GreaterThan(0));
            Assert.That(result2.Id, Is.GreaterThan(0));
            Assert.That(result1.Id, Is.Not.EqualTo(result2.Id));
        }

        [Test]
        public async Task InsertOrder_ShouldReturnOrderWithAssignedId()
        {
            // Arrange
            var newOrder = new Order { Value = 150, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "CreditCard" };

            // Act
            var result = await _repository.InsertOrder(newOrder);

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task InsertOrder_WithSpecificOrderDate_ShouldPreserveOrderDate()
        {
            // Arrange
            var specificDate = new DateTime(2023, 1, 15, 10, 30, 0, DateTimeKind.Utc);
            var newOrder = new Order { Value = 100, CustomerId = 1, OrderDate = specificDate, PaymentMethod = "Pix" };

            // Act
            var result = await _repository.InsertOrder(newOrder);

            // Assert
            Assert.That(result.OrderDate, Is.EqualTo(specificDate));
        }

        [Test]
        public async Task InsertOrder_DifferentValues_ShouldInsertAllCorrectly()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Value = 50.50m, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" },
                new Order { Value = 100.75m, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "CreditCard" },
                new Order { Value = 1000.00m, CustomerId = 2, OrderDate = DateTime.UtcNow, PaymentMethod = "Paypal" }
            };

            // Act
            var results = new List<Order>();
            foreach (var order in orders)
            {
                var result = await _repository.InsertOrder(order);
                results.Add(result);
            }

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].Value, Is.EqualTo(50.50m));
            Assert.That(results[1].Value, Is.EqualTo(100.75m));
            Assert.That(results[2].Value, Is.EqualTo(1000.00m));
        }

        [Test]
        public async Task InsertOrder_ShouldIncrementOrderCountInDatabase()
        {
            // Arrange
            var initialCount = _context.Orders.Count();
            var newOrder = new Order { Value = 200, CustomerId = 1, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" };

            // Act
            await _repository.InsertOrder(newOrder);
            var finalCount = _context.Orders.Count();

            // Assert
            Assert.That(finalCount, Is.EqualTo(initialCount + 1));
        }

        #endregion

        [TearDown]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }
    }
}
