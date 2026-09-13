using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Infra.Data.Context;
using ProvaPub.Infra.Data.Repositories;

namespace ProvaPub.Tests.Repositories
{
    [TestFixture]
    public class CustomerRepositoryTests : IDisposable
    {
        private TestDbContext _context;
        private CustomerRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Arrange - Criar contexto em memória para cada teste
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            _repository = new CustomerRepository(_context);

            // Seed de dados iniciais
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "João Silva", Orders = new List<Order>() },
                new Customer { Id = 2, Name = "Maria Santos", Orders = new List<Order>() },
                new Customer { Id = 3, Name = "Pedro Costa", Orders = new List<Order>() },
                new Customer { Id = 4, Name = "Ana Oliveira", Orders = new List<Order>() },
                new Customer { Id = 5, Name = "Carlos Souza", Orders = new List<Order>() }
            };

            _context.Customers.AddRange(customers);
            _context.SaveChanges();
        }

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCustomer()
        {
            // Arrange
            int customerId = 1;

            // Act
            var result = await _repository.GetByIdAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("João Silva"));
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int invalidId = 999;

            // Act
            var result = await _repository.GetByIdAsync(invalidId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_WithZeroId_ShouldReturnNull()
        {
            // Arrange
            int zeroId = 0;

            // Act
            var result = await _repository.GetByIdAsync(zeroId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_WithNegativeId_ShouldReturnNull()
        {
            // Arrange
            int negativeId = -1;

            // Act
            var result = await _repository.GetByIdAsync(negativeId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_MultipleCallsSameId_ShouldReturnSameCustomer()
        {
            // Arrange
            int customerId = 2;

            // Act
            var result1 = await _repository.GetByIdAsync(customerId);
            var result2 = await _repository.GetByIdAsync(customerId);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.That(result1.Id, Is.EqualTo(result2.Id));
            Assert.That(result1.Name, Is.EqualTo(result2.Name));
        }

        #endregion

        #region GetPagedAsync Tests

        [Test]
        public async Task GetPagedAsync_FirstPage_ShouldReturnFirstPageItems()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;

            // Act
            var (customers, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(customers.Count, Is.EqualTo(2));
            Assert.That(totalCount, Is.EqualTo(5));
            Assert.That(customers[0].Id, Is.EqualTo(1));
            Assert.That(customers[1].Id, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPagedAsync_SecondPage_ShouldReturnSecondPageItems()
        {
            // Arrange
            int page = 2;
            int pageSize = 2;

            // Act
            var (customers, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(customers.Count, Is.EqualTo(2));
            Assert.That(totalCount, Is.EqualTo(5));
            Assert.That(customers[0].Id, Is.EqualTo(3));
            Assert.That(customers[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task GetPagedAsync_LastPage_ShouldReturnRemainingItems()
        {
            // Arrange
            int page = 3;
            int pageSize = 2;

            // Act
            var (customers, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(customers.Count, Is.EqualTo(1));
            Assert.That(totalCount, Is.EqualTo(5));
            Assert.That(customers[0].Id, Is.EqualTo(5));
        }

        [Test]
        public async Task GetPagedAsync_PageOutOfRange_ShouldReturnEmptyList()
        {
            // Arrange
            int page = 10;
            int pageSize = 2;

            // Act
            var (customers, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(customers.Count, Is.EqualTo(0));
            Assert.That(totalCount, Is.EqualTo(5));
        }

        [Test]
        public async Task GetPagedAsync_PageSizeGreaterThanTotal_ShouldReturnAllItems()
        {
            // Arrange
            int page = 1;
            int pageSize = 100;

            // Act
            var (customers, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(customers.Count, Is.EqualTo(5));
            Assert.That(totalCount, Is.EqualTo(5));
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnCorrectTotalCount()
        {
            // Arrange
            int page = 1;
            int pageSize = 1;

            // Act
            var (_, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(totalCount, Is.EqualTo(5));
        }

        [Test]
        public async Task GetPagedAsync_ItemsOrderedById_ShouldBeInCorrectOrder()
        {
            // Arrange
            int page = 1;
            int pageSize = 5;

            // Act
            var (customers, _) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            for (int i = 0; i < customers.Count - 1; i++)
            {
                Assert.That(customers[i].Id, Is.LessThan(customers[i + 1].Id));
            }
        }

        #endregion

        #region HaveBoughtBefore Tests

        [Test]
        public async Task HaveBoughtBefore_CustomerWithOrders_ShouldReturnTrue()
        {
            // Arrange
            var order = new Order 
            { 
                Id = 1,
                Value = 100, 
                CustomerId = 1, 
                OrderDate = DateTime.UtcNow,
                PaymentMethod = "Pix"
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Act
            var result = await _repository.HaveBoughtBefore(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HaveBoughtBefore_CustomerWithoutOrders_ShouldReturnFalse()
        {
            // Arrange
            int customerId = 2;

            // Act
            var result = await _repository.HaveBoughtBefore(customerId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HaveBoughtBefore_NonExistentCustomer_ShouldReturnFalse()
        {
            // Arrange
            int nonExistentCustomerId = 999;

            // Act
            var result = await _repository.HaveBoughtBefore(nonExistentCustomerId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HaveBoughtBefore_CustomerWithMultipleOrders_ShouldReturnTrue()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, Value = 100, CustomerId = 3, OrderDate = DateTime.UtcNow, PaymentMethod = "Pix" },
                new Order { Id = 2, Value = 200, CustomerId = 3, OrderDate = DateTime.UtcNow.AddDays(-1), PaymentMethod = "CreditCard" }
            };
            _context.Orders.AddRange(orders);
            _context.SaveChanges();

            // Act
            var result = await _repository.HaveBoughtBefore(3);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HaveBoughtBefore_WithZeroCustomerId_ShouldReturnFalse()
        {
            // Arrange
            int zeroId = 0;

            // Act
            var result = await _repository.HaveBoughtBefore(zeroId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HaveBoughtBefore_WithNegativeCustomerId_ShouldReturnFalse()
        {
            // Arrange
            int negativeId = -1;

            // Act
            var result = await _repository.HaveBoughtBefore(negativeId);

            // Assert
            Assert.That(result, Is.False);
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
