using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.UseCases.Auth.Login;

public interface ILoginUseCase
{
    Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

