using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Entidade principal que representa um funcionário da empresa.
/// Contém todas as regras de negócio relacionadas ao funcionário.
/// </summary>
public class Employee : Entity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public Role Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    public Guid? ManagerId { get; private set; }
    public Employee? Manager { get; private set; }
    
    private readonly List<Phone> _phones = new();
    public IReadOnlyCollection<Phone> Phones => _phones.AsReadOnly();
    
    private readonly List<Employee> _subordinates = new();
    public IReadOnlyCollection<Employee> Subordinates => _subordinates.AsReadOnly();

    private Employee() { }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string documentNumber,
        string passwordHash,
        DateTime birthDate,
        Role role,
        Guid? managerId = null)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        SetDocumentNumber(documentNumber);
        SetPasswordHash(passwordHash);
        SetBirthDate(birthDate);
        SetRole(role);
        ManagerId = managerId;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("O primeiro nome é obrigatório.", nameof(firstName));

        FirstName = firstName.Trim();
        SetUpdatedAt();
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("O sobrenome é obrigatório.", nameof(lastName));

        LastName = lastName.Trim();
        SetUpdatedAt();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.", nameof(email));

        Email = email.Trim().ToLowerInvariant();
        SetUpdatedAt();
    }

    public void SetDocumentNumber(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException("O número do documento é obrigatório.", nameof(documentNumber));

        DocumentNumber = documentNumber.Trim();
        SetUpdatedAt();
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("A senha é obrigatória.", nameof(passwordHash));

        PasswordHash = passwordHash;
        SetUpdatedAt();
    }

    public void SetBirthDate(DateTime birthDate)
    {
        var age = CalculateAge(birthDate);
        if (age < 18)
            throw new ArgumentException("O funcionário deve ser maior de idade (18 anos ou mais).", nameof(birthDate));

        BirthDate = birthDate;
        SetUpdatedAt();
    }

    public void SetRole(Role role)
    {
        Role = role;
        SetUpdatedAt();
    }

    public void SetManager(Guid? managerId)
    {
        ManagerId = managerId;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void AddPhone(Phone phone)
    {
        if (phone == null)
            throw new ArgumentNullException(nameof(phone));

        _phones.Add(phone);
        SetUpdatedAt();
    }

    public void RemovePhone(Phone phone)
    {
        _phones.Remove(phone);
        SetUpdatedAt();
    }

    public void ClearPhones()
    {
        _phones.Clear();
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se o funcionário atual pode criar outro funcionário com o cargo especificado.
    /// Regra: Não é possível criar um usuário com permissões superiores às do criador.
    /// </summary>
    public bool CanCreateEmployeeWithRole(Role targetRole)
    {
        return Role >= targetRole;
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }
}

