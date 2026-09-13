using Microsoft.EntityFrameworkCore;
using ProvaPub.Domain;
using ProvaPub.Infra.Data.Context;
using ProvaPub.Infra.Data.Repositories;

namespace ProvaPub.Tests.Repositories
{
    [TestFixture]
    public class RandomRepositoryTests : IDisposable
    {
        private TestDbContext _context;
        private RandomRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Arrange - Criar contexto em memória para cada teste
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            _repository = new RandomRepository(_context);
        }

        #region InsertRandomNumber Tests

        [Test]
        public async Task InsertRandomNumber_WithValidNumber_ShouldInsertAndReturn()
        {
            // Arrange
            int number = 42;

            // Act
            var result = await _repository.InsertRandomNumber(number);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Number, Is.EqualTo(42));
        }

        [Test]
        public async Task InsertRandomNumber_ShouldPersistToDatabase()
        {
            // Arrange
            int number = 55;

            // Act
            var result = await _repository.InsertRandomNumber(number);
            var retrievedNumber = await _context.Numbers.FindAsync(result.Id);

            // Assert
            Assert.IsNotNull(retrievedNumber);
            Assert.That(retrievedNumber.Number, Is.EqualTo(55));
        }

        [Test]
        public async Task InsertRandomNumber_MultipleNumbers_ShouldInsertAll()
        {
            // Arrange
            int number1 = 10;
            int number2 = 20;
            int number3 = 30;

            // Act
            var result1 = await _repository.InsertRandomNumber(number1);
            var result2 = await _repository.InsertRandomNumber(number2);
            var result3 = await _repository.InsertRandomNumber(number3);

            // Assert
            Assert.That(result1.Number, Is.EqualTo(10));
            Assert.That(result2.Number, Is.EqualTo(20));
            Assert.That(result3.Number, Is.EqualTo(30));
        }

        [Test]
        public async Task InsertRandomNumber_ShouldReturnWithAssignedId()
        {
            // Arrange
            int number = 77;

            // Act
            var result = await _repository.InsertRandomNumber(number);

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task InsertRandomNumber_WithBoundaryZero_ShouldInsertSuccessfully()
        {
            // Arrange
            int zeroNumber = 0;

            // Act
            var result = await _repository.InsertRandomNumber(zeroNumber);

            // Assert
            Assert.That(result.Number, Is.EqualTo(0));
        }

        [Test]
        public async Task InsertRandomNumber_WithBoundaryNinetynine_ShouldInsertSuccessfully()
        {
            // Arrange
            int maxNumber = 99;

            // Act
            var result = await _repository.InsertRandomNumber(maxNumber);

            // Assert
            Assert.That(result.Number, Is.EqualTo(99));
        }

        [Test]
        public async Task InsertRandomNumber_MultipleCallsSameNumber_ShouldHaveDifferentIds()
        {
            // Arrange
            int number = 50;

            // Act
            var result1 = await _repository.InsertRandomNumber(number);
            var result2 = await _repository.InsertRandomNumber(number);

            // Assert
            Assert.That(result1.Id, Is.Not.EqualTo(result2.Id));
            Assert.That(result1.Number, Is.EqualTo(result2.Number));
        }

        #endregion

        #region RandomNumberExists Tests

        [Test]
        public async Task RandomNumberExists_WithExistingNumber_ShouldReturnTrue()
        {
            // Arrange
            int number = 50;
            await _repository.InsertRandomNumber(number);

            // Act
            var result = await _repository.RandomNumberExists(number);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RandomNumberExists_WithNonExistingNumber_ShouldReturnFalse()
        {
            // Arrange
            int nonExistingNumber = 999;

            // Act
            var result = await _repository.RandomNumberExists(nonExistingNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RandomNumberExists_WithZeroInEmptyDatabase_ShouldReturnFalse()
        {
            // Arrange
            int zeroNumber = 0;

            // Act
            var result = await _repository.RandomNumberExists(zeroNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RandomNumberExists_AfterInsertion_ShouldReturnTrue()
        {
            // Arrange
            int number = 77;

            // Act
            await _repository.InsertRandomNumber(number);
            var result = await _repository.RandomNumberExists(number);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RandomNumberExists_WithMultipleNumbers_ShouldReturnCorrectResult()
        {
            // Arrange
            int[] numbers = { 10, 25, 50, 75 };
            foreach (var number in numbers)
            {
                await _repository.InsertRandomNumber(number);
            }

            // Act & Assert
            Assert.That(await _repository.RandomNumberExists(10), Is.True);
            Assert.That(await _repository.RandomNumberExists(25), Is.True);
            Assert.That(await _repository.RandomNumberExists(50), Is.True);
            Assert.That(await _repository.RandomNumberExists(75), Is.True);
            Assert.That(await _repository.RandomNumberExists(99), Is.False);
        }

        [Test]
        public async Task RandomNumberExists_CheckingMultipleTimesForSameNumber_ShouldBeConsistent()
        {
            // Arrange
            int number = 42;
            await _repository.InsertRandomNumber(number);

            // Act
            var result1 = await _repository.RandomNumberExists(number);
            var result2 = await _repository.RandomNumberExists(number);
            var result3 = await _repository.RandomNumberExists(number);

            // Assert
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.True);
        }

        [Test]
        public async Task RandomNumberExists_WithZeroBoundary_ShouldWorkCorrectly()
        {
            // Arrange
            int zeroNumber = 0;
            await _repository.InsertRandomNumber(zeroNumber);

            // Act
            var result = await _repository.RandomNumberExists(zeroNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RandomNumberExists_WithNinetynineeBoundary_ShouldWorkCorrectly()
        {
            // Arrange
            int maxNumber = 99;
            await _repository.InsertRandomNumber(maxNumber);

            // Act
            var result = await _repository.RandomNumberExists(maxNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RandomNumberExists_WithNegativeNumber_ShouldReturnFalse()
        {
            // Arrange
            int negativeNumber = -5;

            // Act
            var result = await _repository.RandomNumberExists(negativeNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RandomNumberExists_WithNumberGreaterThanNinetynine_ShouldReturnFalse()
        {
            // Arrange
            int largeNumber = 100;

            // Act
            var result = await _repository.RandomNumberExists(largeNumber);

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
