using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.UseCases.Auth.Login;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests.Application.UseCases.Auth;

/// <summary>
/// Testes unitários para o LoginUseCase.
/// </summary>
public class LoginUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<LoginUseCase>> _loggerMock;
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<LoginUseCase>>();

        _sut = new LoginUseCase(
            _employeeRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var request = new LoginRequest("joao.silva@empresa.com", "Senha@123");
        var employee = CreateValidEmployee();
        var token = "jwt-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var employeeResponse = CreateEmployeeResponse(employee);

        _employeeRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _passwordHasherMock
            .Setup(x => x.Verify(request.Password, employee.PasswordHash))
            .Returns(true);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(employee))
            .Returns((token, expiresAt));

        _mapperMock
            .Setup(x => x.Map<EmployeeResponse>(employee))
            .Returns(employeeResponse);

        // Act
        var result = await _sut.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.ExpiresAt.Should().Be(expiresAt);
        result.Employee.Should().NotBeNull();
        result.Employee.Email.Should().Be(employee.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new LoginRequest("naoexiste@empresa.com", "Senha@123");

        _employeeRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*E-mail ou senha inválidos*");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new LoginRequest("joao.silva@empresa.com", "SenhaErrada");
        var employee = CreateValidEmployee();

        _employeeRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _passwordHasherMock
            .Setup(x => x.Verify(request.Password, employee.PasswordHash))
            .Returns(false);

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*E-mail ou senha inválidos*");
    }

    [Fact]
    public async Task ExecuteAsync_WithInactiveUser_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new LoginRequest("joao.silva@empresa.com", "Senha@123");
        var employee = CreateValidEmployee();
        employee.Deactivate();

        _employeeRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*inativo*");
    }

    private static Employee CreateValidEmployee()
    {
        var employee = new Employee(
            firstName: "João",
            lastName: "Silva",
            email: "joao.silva@empresa.com",
            documentNumber: "12345678900",
            passwordHash: "hashedpassword",
            birthDate: DateTime.Today.AddYears(-25),
            role: Role.Employee
        );

        employee.AddPhone(new Phone("11999999999", "Celular"));

        return employee;
    }

    private static EmployeeResponse CreateEmployeeResponse(Employee employee)
    {
        return new EmployeeResponse(
            Id: employee.Id,
            FirstName: employee.FirstName,
            LastName: employee.LastName,
            FullName: employee.FullName,
            Email: employee.Email,
            DocumentNumber: employee.DocumentNumber,
            BirthDate: employee.BirthDate,
            Role: employee.Role,
            IsActive: employee.IsActive,
            ManagerId: employee.ManagerId,
            ManagerName: null,
            Phones: employee.Phones.Select(p => new PhoneDto(p.Id, p.Number, p.Type)).ToList(),
            CreatedAt: employee.CreatedAt,
            UpdatedAt: employee.UpdatedAt
        );
    }
}
