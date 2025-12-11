namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO para requisição de login.
/// </summary>
public record LoginRequest(
    string Email,
    string Password
);

