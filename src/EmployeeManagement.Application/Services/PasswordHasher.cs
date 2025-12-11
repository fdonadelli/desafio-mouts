using EmployeeManagement.Application.Interfaces;

namespace EmployeeManagement.Application.Services;

/// <summary>
/// Implementação do serviço de hash de senhas usando BCrypt.
/// BCrypt é uma escolha segura pois:
/// - Inclui salt automaticamente
/// - É computacionalmente caro (resistente a ataques de força bruta)
/// - O work factor pode ser ajustado conforme necessário
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

