using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.GetSubordinates;

public interface IGetSubordinatesUseCase
{
    Task<IEnumerable<EmployeeResponse>> ExecuteAsync(Guid managerId, CancellationToken cancellationToken = default);
}

