namespace EmployeeManagement.Application.UseCases.Employees.Delete;

public interface IDeleteEmployeeUseCase
{
    Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

