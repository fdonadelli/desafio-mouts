namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Interface para serviço de hash de senhas.
/// Abstrai a implementação do algoritmo de hash.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

