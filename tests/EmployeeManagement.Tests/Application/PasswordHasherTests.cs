using EmployeeManagement.Application.Services;
using FluentAssertions;

namespace EmployeeManagement.Tests.Application;

/// <summary>
/// Testes unitários para o PasswordHasher.
/// </summary>
public class PasswordHasherTests
{
    private readonly PasswordHasher _sut;

    public PasswordHasherTests()
    {
        _sut = new PasswordHasher();
    }

    [Fact]
    public void Hash_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "Senha@123";

        // Act
        var hash = _sut.Hash(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$2"); // BCrypt hash prefix
    }

    [Fact]
    public void Hash_ShouldReturnDifferentHashForSamePassword()
    {
        // Arrange
        var password = "Senha@123";

        // Act
        var hash1 = _sut.Hash(password);
        var hash2 = _sut.Hash(password);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt gera salt diferente a cada vez
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Senha@123";
        var hash = _sut.Hash(password);

        // Act
        var result = _sut.Verify(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Senha@123";
        var wrongPassword = "SenhaErrada";
        var hash = _sut.Hash(password);

        // Act
        var result = _sut.Verify(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }
}

