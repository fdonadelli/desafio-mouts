using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO de resposta com dados do funcionário.
/// </summary>
public record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string DocumentNumber,
    DateTime BirthDate,
    Role Role,
    bool IsActive,
    Guid? ManagerId,
    string? ManagerName,
    List<PhoneDto> Phones,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

