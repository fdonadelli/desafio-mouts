namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO para alteração de senha.
/// </summary>
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

