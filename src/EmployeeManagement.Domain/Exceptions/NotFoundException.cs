namespace EmployeeManagement.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key) 
        : base($"{entityName} com identificador '{key}' não foi encontrado.")
    {
    }
}

