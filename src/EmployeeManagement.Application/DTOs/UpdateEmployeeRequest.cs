using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO para atualização de um funcionário existente.
/// </summary>
public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime BirthDate,
    Role Role,
    Guid? ManagerId,
    List<PhoneDto> Phones
);

