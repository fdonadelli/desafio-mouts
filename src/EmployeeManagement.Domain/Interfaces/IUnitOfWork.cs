namespace EmployeeManagement.Domain.Interfaces;

/// <summary>
/// Interface Unit of Work.
/// Padrão Unit of Work - garante que todas as operações sejam commitadas em uma única transação.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

