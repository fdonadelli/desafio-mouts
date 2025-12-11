namespace EmployeeManagement.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

