using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Infra.Data.Context;
using ProvaPub.Infra.Data.Repositories;

namespace ProvaPub.Tests.Repositories
{
    [TestFixture]
    public class ProductRepositoryTests : IDisposable
    {
        private TestDbContext _context;
        private ProductRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Arrange - Criar contexto em memória para cada teste
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            _repository = new ProductRepository(_context);

            // Seed de dados iniciais
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Produto A" },
                new Product { Id = 2, Name = "Produto B" },
                new Product { Id = 3, Name = "Produto C" },
                new Product { Id = 4, Name = "Produto D" },
                new Product { Id = 5, Name = "Produto E" },
                new Product { Id = 6, Name = "Produto F" },
                new Product { Id = 7, Name = "Produto G" },
                new Product { Id = 8, Name = "Produto H" }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        #region GetPagedAsync Tests

        [Test]
        public async Task GetPagedAsync_FirstPage_ShouldReturnFirstPageItems()
        {
            // Arrange
            int page = 1;
            int pageSize = 3;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(3));
            Assert.That(totalCount, Is.EqualTo(8));
            Assert.That(products[0].Id, Is.EqualTo(1));
            Assert.That(products[1].Id, Is.EqualTo(2));
            Assert.That(products[2].Id, Is.EqualTo(3));
        }

        [Test]
        public async Task GetPagedAsync_SecondPage_ShouldReturnSecondPageItems()
        {
            // Arrange
            int page = 2;
            int pageSize = 3;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(3));
            Assert.That(totalCount, Is.EqualTo(8));
            Assert.That(products[0].Id, Is.EqualTo(4));
            Assert.That(products[1].Id, Is.EqualTo(5));
            Assert.That(products[2].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task GetPagedAsync_LastPage_ShouldReturnRemainingItems()
        {
            // Arrange
            int page = 3;
            int pageSize = 3;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(2));
            Assert.That(totalCount, Is.EqualTo(8));
            Assert.That(products[0].Id, Is.EqualTo(7));
            Assert.That(products[1].Id, Is.EqualTo(8));
        }

        [Test]
        public async Task GetPagedAsync_PageOutOfRange_ShouldReturnEmptyList()
        {
            // Arrange
            int page = 10;
            int pageSize = 3;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(0));
            Assert.That(totalCount, Is.EqualTo(8));
        }

        [Test]
        public async Task GetPagedAsync_PageSizeGreaterThanTotal_ShouldReturnAllItems()
        {
            // Arrange
            int page = 1;
            int pageSize = 100;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(8));
            Assert.That(totalCount, Is.EqualTo(8));
        }

        [Test]
        public async Task GetPagedAsync_PageSizeOne_ShouldReturnOneItemPerPage()
        {
            // Arrange
            int page = 1;
            int pageSize = 1;

            // Act
            var (products, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(1));
            Assert.That(totalCount, Is.EqualTo(8));
            Assert.That(products[0].Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnCorrectTotalCount()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;

            // Act
            var (_, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(totalCount, Is.EqualTo(8));
        }

        [Test]
        public async Task GetPagedAsync_ItemsOrderedById_ShouldBeInCorrectOrder()
        {
            // Arrange
            int page = 1;
            int pageSize = 8;

            // Act
            var (products, _) = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            for (int i = 0; i < products.Count - 1; i++)
            {
                Assert.That(products[i].Id, Is.LessThan(products[i + 1].Id));
            }
        }

        [Test]
        public async Task GetPagedAsync_MultiplePages_ShouldMaintainOrder()
        {
            // Arrange
            int pageSize = 3;

            // Act
            var (page1Products, _) = await _repository.GetPagedAsync(1, pageSize);
            var (page2Products, _) = await _repository.GetPagedAsync(2, pageSize);

            // Assert
            Assert.That(page1Products[page1Products.Count - 1].Id, Is.LessThan(page2Products[0].Id));
        }

        [Test]
        public async Task GetPagedAsync_WithEmptyDatabase_ShouldReturnZeroItems()
        {
            // Arrange
            var emptyContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);
            var emptyRepository = new ProductRepository(emptyContext);

            int page = 1;
            int pageSize = 10;

            // Act
            var (products, totalCount) = await emptyRepository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.That(products.Count, Is.EqualTo(0));
            Assert.That(totalCount, Is.EqualTo(0));

            // Cleanup
            emptyContext.Dispose();
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
