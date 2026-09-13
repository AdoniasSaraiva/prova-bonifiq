namespace ProvaPub.Infra.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Application.Interfaces.Strategy;
using ProvaPub.Infra.Data.Repositories;
using ProvaPub.Infra.PaymentMethod.PaymentStrategyMethod;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositórios
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRandomRepository, RandomRepository>();

        // Estratégias de Pagamento Concretas
        services.AddScoped<IPaymentStrategy, PixPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, PaypalPaymentStrategy>();

        return services;
    }
}