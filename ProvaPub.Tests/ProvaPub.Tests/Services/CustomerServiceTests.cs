namespace ProvaPub.Tests.Services;

using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProvaPub.Application.Exceptions;
using ProvaPub.Application.Interfaces.DataFormatProvider;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Application.Services;
using ProvaPub.Domain;
using ProvaPub.Domain.Service;

[TestFixture]
public class CustomerServiceTests
{
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IDateTimeProvider> _dateTimeProviderMock;
    private Mock<PurchasePolicy> _purchasePolicy;
    private CustomerService _customerService;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _purchasePolicy = new Mock<PurchasePolicy>();

        _customerService = new CustomerService(
            _customerRepositoryMock.Object,
            _orderRepositoryMock.Object,
            new PurchasePolicy(),
            _dateTimeProviderMock.Object
        );
    }

    [Test]
    public async Task CanPurchase_WhenCustomerIdIsZero_ShouldReturnFalseWithoutHittingDatabase()
    {
        // Arrange: ID zerado
        var customerId = 0;
        var purchaseValue = 50m;

        // Act
        var ex = Assert.ThrowsAsync<BusinessException>(async () => await _customerService.CanPurchase(customerId, purchaseValue));

        // Assert

        Assert.That(ex.Message, Is.EqualTo("O valor da propriedade 'CustomerId' não pode ser 0 ou menor que 0."));

        // Garante que nenhum repositório foi consultado
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.HasPurchaseThisMonth(It.IsAny<int>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.ManyTimesCustomerPurchaseInMonth(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Test]
    public async Task CanPurchase_WhenPurchaseValueIsZero_ShouldReturnFalseWithoutHittingDatabase()
    {
        // Arrange: Valor de compra zerado
        var customerId = 1;
        var purchaseValue = 0m;

        // Act
        var ex = Assert.ThrowsAsync<BusinessException>(async () => await _customerService.CanPurchase(customerId, purchaseValue));

        // Assert

        Assert.That(ex.Message, Is.EqualTo("O valor da propriedade 'PurchaseValue' não pode ser 0 ou menor que 0."));

        // Garante que nenhum repositório foi consultado
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.HasPurchaseThisMonth(It.IsAny<int>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.ManyTimesCustomerPurchaseInMonth(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Test]
    public async Task CanPurchase_WhenCustomerDoesNotExist_ShouldThrowBusinessException()
    {
        // Arrange: ID de um cliente inexistente
        var customerId = 999;
        var purchaseValue = 50m;

        // Configura o repositório para retornar null ao buscar o cliente
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null!);

        // Act & Assert: Verifica o disparo da exceção de negócio
        var ex = Assert.ThrowsAsync<BusinessException>(async () =>
            await _customerService.CanPurchase(customerId, purchaseValue));

        // Assert: Valida a mensagem da exceção e que não prosseguiu com consultas adicionais
        Assert.That(ex.Message, Is.EqualTo("Cliente não encontrado."));
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _orderRepositoryMock.Verify(x => x.HasPurchaseThisMonth(It.IsAny<int>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.ManyTimesCustomerPurchaseInMonth(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Test]
    public async Task CanPurchase_WhenAllConditionsAreValid_ShouldReturnTrue()
    {
        // Arrange
        var customerId = 1;
        var purchaseValue = 85.50m; 

        // 1. Simula horário comercial válido (ex: Segunda-feira às 14:00)
        var businessHours = new DateTime(2026, 9, 14, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(businessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.True);

        // Valida que as consultas necessárias foram executadas
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _orderRepositoryMock.Verify(x => x.HasPurchaseThisMonth(customerId), Times.Once);
    }

    [Test]
    public async Task CanPurchase_WhenOutsideBusinessHours_ShouldReturnFalse()
    {
        // Arrange
        var customerId = 1;
        var purchaseValue = 85.50m;

        // Simula horário fora do expediente comercial (Domingo às 22:00)
        var outsideBusinessHours = new DateTime(2026, 9, 13, 22, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(outsideBusinessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CanPurchase_WhenCustomerAlreadyPurchasedThisMonth_ShouldReturnFalse()
    {
        // Arrange
        var customerId = 1;
        var purchaseValue = 85.50m;

        // Simula horário comercial válido
        var businessHours = new DateTime(2026, 9, 14, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(businessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        // Simula que o cliente JÁ realizou uma compra neste mês
        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CanPurchase_WhenFirstPurchaseIsGreaterThan100_ShouldReturnFalse()
    {
        // Arrange
        var customerId = 1;
        var purchaseValue = 150.00m; // Primeira compra com valor acima de R$ 100,00

        // Simula horário comercial válido
        var businessHours = new DateTime(2026, 9, 14, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(businessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        // Simula que NUNCA comprou antes
        _customerRepositoryMock
            .Setup(x => x.HaveBoughtBefore(customerId))
            .ReturnsAsync(false);

        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CanPurchase_WhenCustomerIdIsNegative_ShouldThrowBusinessExceptionWithoutHittingDatabase()
    {
        // Arrange
        var customerId = -1;
        var purchaseValue = 50m;

        // Act & Assert
        var ex = Assert.ThrowsAsync<BusinessException>(async () =>
            await _customerService.CanPurchase(customerId, purchaseValue));

        Assert.That(ex.Message, Is.EqualTo("O valor da propriedade 'CustomerId' não pode ser 0 ou menor que 0."));
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task CanPurchase_WhenPurchaseValueIsNegative_ShouldThrowBusinessExceptionWithoutHittingDatabase()
    {
        // Arrange
        var customerId = 1;
        var purchaseValue = -10m;

        // Act & Assert
        var ex = Assert.ThrowsAsync<BusinessException>(async () =>
            await _customerService.CanPurchase(customerId, purchaseValue));

        Assert.That(ex.Message, Is.EqualTo("O valor da propriedade 'PurchaseValue' não pode ser 0 ou menor que 0."));
        _customerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task CanPurchase_WhenFirstPurchaseIsExactly100_ShouldReturnTrue()
    {
        var customerId = 1;
        var purchaseValue = 100.00m;

        var businessHours = new DateTime(2026, 9, 14, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(businessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        _customerRepositoryMock
            .Setup(x => x.HaveBoughtBefore(customerId))
            .ReturnsAsync(false);

        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanPurchase_WhenCustomerHasBoughtBeforeAndValueIsGreaterThan100_ShouldReturnTrue()
    {
        var customerId = 1;
        var purchaseValue = 500.00m;

        var businessHours = new DateTime(2026, 9, 14, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(businessHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(new Customer { Id = customerId, Name = "Adonias" });

        // JÁ comprou antes
        _customerRepositoryMock
            .Setup(x => x.HaveBoughtBefore(customerId))
            .ReturnsAsync(true);

        _orderRepositoryMock
            .Setup(x => x.HasPurchaseThisMonth(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _customerService.CanPurchase(customerId, purchaseValue);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanPurchase_WhenOnSaturdayOrSunday_ShouldReturnFalse()
    {
        var saturdayHours = new DateTime(2026, 9, 12, 14, 0, 0);
        _dateTimeProviderMock?.Setup(x => x.UtcNow).Returns(saturdayHours);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Customer { Id = 1, Name = "Adonias" });

        // Act
        var result = await _customerService.CanPurchase(1, 50m);

        // Assert
        Assert.That(result, Is.False);
    }

}