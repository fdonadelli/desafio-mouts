using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Employees.ChangePassword;

public interface IChangePasswordUseCase
{
    Task ExecuteAsync(Guid id, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}

