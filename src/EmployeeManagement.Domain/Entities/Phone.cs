namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Representa um telefone de contato do funcionário.
/// Um funcionário pode ter múltiplos telefones.
/// </summary>
public class Phone : Entity
{
    public string Number { get; private set; } = string.Empty;
    public string? Type { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    private Phone() { }

    public Phone(string number, string? type = null)
    {
        SetNumber(number);
        Type = type;
    }

    public void SetNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("O número de telefone é obrigatório.", nameof(number));

        Number = number.Trim();
        SetUpdatedAt();
    }

    public void SetType(string? type)
    {
        Type = type?.Trim();
        SetUpdatedAt();
    }
}

