using EmployeeManagement.Domain.Entities;
using FluentAssertions;

namespace EmployeeManagement.Tests.Domain;

/// <summary>
/// Testes unitários para a entidade Phone.
/// </summary>
public class PhoneTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePhone()
    {
        // Arrange
        var number = "11999999999";
        var type = "Celular";

        // Act
        var phone = new Phone(number, type);

        // Assert
        phone.Number.Should().Be(number);
        phone.Type.Should().Be(type);
        phone.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyNumber_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Phone("", "Celular");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*telefone*");
    }

    [Fact]
    public void Constructor_WithNullType_ShouldCreatePhone()
    {
        // Arrange & Act
        var phone = new Phone("11999999999");

        // Assert
        phone.Number.Should().Be("11999999999");
        phone.Type.Should().BeNull();
    }

    [Fact]
    public void SetNumber_WithValidNumber_ShouldUpdateNumber()
    {
        // Arrange
        var phone = new Phone("11999999999");

        // Act
        phone.SetNumber("11888888888");

        // Assert
        phone.Number.Should().Be("11888888888");
    }

    [Fact]
    public void SetType_ShouldUpdateType()
    {
        // Arrange
        var phone = new Phone("11999999999", "Celular");

        // Act
        phone.SetType("Comercial");

        // Assert
        phone.Type.Should().Be("Comercial");
    }
}

