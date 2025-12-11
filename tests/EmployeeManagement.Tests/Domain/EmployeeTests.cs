using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using FluentAssertions;

namespace EmployeeManagement.Tests.Domain;

/// <summary>
/// Testes unitários para a entidade Employee.
/// Testa as regras de negócio do domínio.
/// </summary>
public class EmployeeTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var email = "joao.silva@empresa.com";
        var documentNumber = "12345678900";
        var passwordHash = "hashedpassword";
        var birthDate = DateTime.Today.AddYears(-25);
        var role = Role.Employee;

        // Act
        var employee = new Employee(firstName, lastName, email, documentNumber, passwordHash, birthDate, role);

        // Assert
        employee.FirstName.Should().Be(firstName);
        employee.LastName.Should().Be(lastName);
        employee.Email.Should().Be(email.ToLowerInvariant());
        employee.DocumentNumber.Should().Be(documentNumber);
        employee.PasswordHash.Should().Be(passwordHash);
        employee.BirthDate.Should().Be(birthDate);
        employee.Role.Should().Be(role);
        employee.IsActive.Should().BeTrue();
        employee.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithMinorBirthDate_ShouldThrowArgumentException()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-17); // 17 anos

        // Act
        var act = () => new Employee(
            "João", "Silva", "joao@empresa.com", "12345678900",
            "hash", birthDate, Role.Employee);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*maior de idade*");
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Employee(
            "", "Silva", "joao@empresa.com", "12345678900",
            "hash", DateTime.Today.AddYears(-25), Role.Employee);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*primeiro nome*");
    }

    [Fact]
    public void Constructor_WithEmptyLastName_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Employee(
            "João", "", "joao@empresa.com", "12345678900",
            "hash", DateTime.Today.AddYears(-25), Role.Employee);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*sobrenome*");
    }

    [Fact]
    public void Constructor_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Employee(
            "João", "Silva", "", "12345678900",
            "hash", DateTime.Today.AddYears(-25), Role.Employee);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*e-mail*");
    }

    [Fact]
    public void FullName_ShouldReturnFirstNameAndLastName()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        var fullName = employee.FullName;

        // Assert
        fullName.Should().Be("João Silva");
    }

    [Theory]
    [InlineData(Role.Director, Role.Director, true)]
    [InlineData(Role.Director, Role.Leader, true)]
    [InlineData(Role.Director, Role.Employee, true)]
    [InlineData(Role.Leader, Role.Leader, true)]
    [InlineData(Role.Leader, Role.Employee, true)]
    [InlineData(Role.Leader, Role.Director, false)]
    [InlineData(Role.Employee, Role.Employee, true)]
    [InlineData(Role.Employee, Role.Leader, false)]
    [InlineData(Role.Employee, Role.Director, false)]
    public void CanCreateEmployeeWithRole_ShouldReturnExpectedResult(
        Role creatorRole, Role targetRole, bool expectedResult)
    {
        // Arrange
        var employee = CreateValidEmployee(creatorRole);

        // Act
        var result = employee.CanCreateEmployeeWithRole(targetRole);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var employee = CreateValidEmployee();
        employee.IsActive.Should().BeTrue();

        // Act
        employee.Deactivate();

        // Assert
        employee.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var employee = CreateValidEmployee();
        employee.Deactivate();

        // Act
        employee.Activate();

        // Assert
        employee.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddPhone_ShouldAddPhoneToCollection()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var phone = new Phone("11999999999", "Celular");

        // Act
        employee.AddPhone(phone);

        // Assert
        employee.Phones.Should().HaveCount(1);
        employee.Phones.First().Number.Should().Be("11999999999");
    }

    [Fact]
    public void RemovePhone_ShouldRemovePhoneFromCollection()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var phone = new Phone("11999999999", "Celular");
        employee.AddPhone(phone);

        // Act
        employee.RemovePhone(phone);

        // Assert
        employee.Phones.Should().BeEmpty();
    }

    [Fact]
    public void SetEmail_ShouldConvertToLowerCase()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act
        employee.SetEmail("JOAO.SILVA@EMPRESA.COM");

        // Assert
        employee.Email.Should().Be("joao.silva@empresa.com");
    }

    private static Employee CreateValidEmployee(Role role = Role.Employee)
    {
        return new Employee(
            firstName: "João",
            lastName: "Silva",
            email: "joao.silva@empresa.com",
            documentNumber: "12345678900",
            passwordHash: "hashedpassword",
            birthDate: DateTime.Today.AddYears(-25),
            role: role
        );
    }
}

