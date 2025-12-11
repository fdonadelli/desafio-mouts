using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Mappings;

/// <summary>
/// Classe estática para mapeamento entre entidades e DTOs.
/// Evita dependência de bibliotecas externas de mapeamento para casos simples.
/// </summary>
public static class EmployeeMapper
{
    public static EmployeeResponse ToResponse(Employee employee)
    {
        return new EmployeeResponse(
            Id: employee.Id,
            FirstName: employee.FirstName,
            LastName: employee.LastName,
            FullName: employee.FullName,
            Email: employee.Email,
            DocumentNumber: employee.DocumentNumber,
            BirthDate: employee.BirthDate,
            Role: employee.Role,
            IsActive: employee.IsActive,
            ManagerId: employee.ManagerId,
            ManagerName: employee.Manager?.FullName,
            Phones: employee.Phones.Select(p => new PhoneDto(p.Id, p.Number, p.Type)).ToList(),
            CreatedAt: employee.CreatedAt,
            UpdatedAt: employee.UpdatedAt
        );
    }

    public static IEnumerable<EmployeeResponse> ToResponseList(IEnumerable<Employee> employees)
    {
        return employees.Select(ToResponse);
    }
}

