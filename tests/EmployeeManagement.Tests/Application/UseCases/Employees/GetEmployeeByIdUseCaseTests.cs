using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.UseCases.Employees.GetById;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests.Application.UseCases.Employees;

/// <summary>
/// Testes unitários para o GetEmployeeByIdUseCase.
/// </summary>
public class GetEmployeeByIdUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetEmployeeByIdUseCase>> _loggerMock;
    private readonly GetEmployeeByIdUseCase _sut;

    public GetEmployeeByIdUseCaseTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetEmployeeByIdUseCase>>();

        _sut = new GetEmployeeByIdUseCase(
            _employeeRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeExists_ShouldReturnEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = CreateValidEmployee(employeeId);
        var employeeResponse = CreateEmployeeResponse(employee);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdWithPhonesAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _mapperMock
            .Setup(x => x.Map<EmployeeResponse>(employee))
            .Returns(employeeResponse);

        // Act
        var result = await _sut.ExecuteAsync(employeeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employeeId);
        result.Email.Should().Be(employee.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmployeeNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        _employeeRepositoryMock
            .Setup(x => x.GetByIdWithPhonesAsync(employeeId, It.IsAny<CancellationToken>()))
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
