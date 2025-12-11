namespace EmployeeManagement.Domain.Enums;

/// <summary>
/// Representa os níveis hierárquicos de um funcionário na empresa.
/// A ordem numérica define a hierarquia (maior = mais permissões).
/// </summary>
public enum Role
{
    Employee = 1,
    Leader = 2,
    Director = 3
}

