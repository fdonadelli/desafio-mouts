using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.Create;

public interface ICreateEmployeeUseCase
{
    Task<EmployeeResponse> ExecuteAsync(CreateEmployeeRequest request, Guid creatorId, CancellationToken cancellationToken = default);
}

