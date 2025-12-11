using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Interface para serviço de geração de tokens JWT.
/// </summary>
public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Employee employee);
}

