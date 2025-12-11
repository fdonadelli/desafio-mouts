using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.UseCases.Employees.ChangePassword;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests.Application.UseCases.Employees;

/// <summary>
/// Testes unitários para o ChangePasswordUseCase.
/// </summary>
public class ChangePasswordUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ILogger<ChangePasswordUseCase>> _loggerMock;
    private readonly ChangePasswordUseCase _sut;

    public ChangePasswordUseCaseTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loggerMock = new Mock<ILogger<ChangePasswordUseCase>>();

        _sut = new ChangePasswordUseCase(
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCurrentPasswordIsCorrect_ShouldUpdatePassword()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = CreateValidEmployee(employeeId);
        var request = new ChangePasswordRequest("CurrentPassword", "NewPassword@123");

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _passwordHasherMock
            .Setup(x => x.Verify(request.CurrentPassword, employee.PasswordHash))
            .Returns(true);

        _passwordHasherMock
            .Setup(x => x.Hash(request.NewPassword))
            .Returns("newhashedpassword");

        // Act
        await _sut.ExecuteAsync(employeeId, request);

        // Assert
        _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCurrentPasswordIsIncorrect_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = CreateValidEmployee(employeeId);
        var request = new ChangePasswordRequest("WrongPassword", "NewPassword@123");

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _passwordHasherMock
            .Setup(x => x.Verify(request.CurrentPassword, employee.PasswordHash))
            .Returns(false);

        // Act
        var act = async () => await _sut.ExecuteAsync(employeeId, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*senha atual*incorreta*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new ChangePasswordRequest("CurrentPassword", "NewPassword@123");

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var act = async () => await _sut.ExecuteAsync(employeeId, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    private static Employee CreateValidEmployee(Guid id, Role role = Role.Employee)
    {
        var employee = new Employee(
            firstName: "João",
            lastName: "Silva",
            email: "joao.silva@empresa.com",
            documentNumber: "12345678900",
            passwordHash: "hashedpassword",
            birthDate: DateTime.Today.AddYears(-25),
            role: role
        );

        typeof(Entity).GetProperty("Id")!.SetValue(employee, id);
        employee.AddPhone(new Phone("11999999999", "Celular"));

        return employee;
    }
}

