using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Interfaces;

/// <summary>
/// Interface do repositório de funcionários.
/// Define o contrato para operações de persistência.
/// Padrão Repository - abstrai a camada de dados do domínio.
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithPhonesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Employee?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetAllWithPhonesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    void Update(Employee employee);
    void Delete(Employee employee);
}

