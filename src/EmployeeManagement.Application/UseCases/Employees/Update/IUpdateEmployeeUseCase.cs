using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.Update;

public interface IUpdateEmployeeUseCase
{
    Task<EmployeeResponse> ExecuteAsync(Guid id, UpdateEmployeeRequest request, Guid updaterId, CancellationToken cancellationToken = default);
}

