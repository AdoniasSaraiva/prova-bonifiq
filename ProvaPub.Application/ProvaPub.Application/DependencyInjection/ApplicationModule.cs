namespace ProvaPub.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using ProvaPub.Application.Interfaces.DataFormatProvider;
using ProvaPub.Application.Interfaces.Services;
using ProvaPub.Application.Services;
using ProvaPub.Application.Services.DataFormatProvider;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Serviços de Aplicação / Casos de Uso
        services.AddScoped<IRandomService, RandomService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped< IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}