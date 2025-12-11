namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO de resposta do login com token JWT.
/// </summary>
public record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    EmployeeResponse Employee
);

