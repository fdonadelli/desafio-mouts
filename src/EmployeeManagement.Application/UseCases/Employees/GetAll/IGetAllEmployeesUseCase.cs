using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.GetAll;

public interface IGetAllEmployeesUseCase
{
    Task<IEnumerable<EmployeeResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}

