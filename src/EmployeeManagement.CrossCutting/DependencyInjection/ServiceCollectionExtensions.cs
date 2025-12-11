using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.CrossCutting.DependencyInjection;

/// <summary>
/// Extensão principal para registro de todos os serviços da aplicação.
/// Centraliza a configuração de DI respeitando o Dependency Inversion Principle.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona todos os serviços da aplicação (Application + Infrastructure).
    /// </summary>
    public static IServiceCollection AddEmployeeManagement(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        return services;
    }
}

