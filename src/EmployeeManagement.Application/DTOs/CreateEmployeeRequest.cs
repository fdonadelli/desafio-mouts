using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// DTO para criação de um novo funcionário.
/// </summary>
public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string DocumentNumber,
    string Password,
    DateTime BirthDate,
    Role Role,
    Guid? ManagerId,
    List<PhoneDto> Phones
);

