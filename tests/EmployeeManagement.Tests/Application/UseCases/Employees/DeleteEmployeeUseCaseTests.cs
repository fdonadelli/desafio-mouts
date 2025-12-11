using EmployeeManagement.Application.UseCases.Employees.Delete;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests.Application.UseCases.Employees;

/// <summary>
/// Testes unitários para o DeleteEmployeeUseCase.
/// </summary>
public class DeleteEmployeeUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteEmployeeUseCase>> _loggerMock;
    private readonly DeleteEmployeeUseCase _sut;

    public DeleteEmployeeUseCaseTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteEmployeeUseCase>>();

        _sut = new DeleteEmployeeUseCase(
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeExists_ShouldDeactivateEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = CreateValidEmployee(employeeId);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        await _sut.ExecuteAsync(employeeId);

        // Assert
        employee.IsActive.Should().BeFalse();
        _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var act = async () => await _sut.ExecuteAsync(employeeId);

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

