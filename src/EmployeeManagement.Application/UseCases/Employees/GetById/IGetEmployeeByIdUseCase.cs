using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.GetById;

public interface IGetEmployeeByIdUseCase
{
    Task<EmployeeResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

