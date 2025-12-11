using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.UseCases.Employees.Create;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests.Application.UseCases.Employees;

/// <summary>
/// Testes unitários para o CreateEmployeeUseCase.
/// </summary>
public class CreateEmployeeUseCaseTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateEmployeeUseCase>> _loggerMock;
    private readonly CreateEmployeeUseCase _sut;

    public CreateEmployeeUseCaseTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateEmployeeUseCase>>();

        _sut = new CreateEmployeeUseCase(
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCreatorHasPermission_ShouldCreateEmployee()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var creator = CreateValidEmployee(creatorId, Role.Director);

        var request = new CreateEmployeeRequest(
            FirstName: "Maria",
            LastName: "Santos",
            Email: "maria.santos@empresa.com",
            DocumentNumber: "98765432100",
            Password: "Senha@123",
            BirthDate: DateTime.Today.AddYears(-25),
            Role: Role.Employee,
            ManagerId: null,
            Phones: new List<PhoneDto> { new(null, "11999999999", "Celular") }
        );

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        _employeeRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.ExistsByDocumentNumberAsync(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.Hash(request.Password))
            .Returns("hashedpassword");

        _employeeRepositoryMock
            .Setup(x => x.GetByIdWithPhonesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => CreateEmployeeFromRequest(id, request));

        _mapperMock
            .Setup(x => x.Map<EmployeeResponse>(It.IsAny<Employee>()))
            .Returns((Employee emp) => CreateEmployeeResponse(emp));

        // Act
        var result = await _sut.ExecuteAsync(request, creatorId);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email.ToLowerInvariant());

        _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCreatorHasNoPermission_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var creator = CreateValidEmployee(creatorId, Role.Employee);

        var request = new CreateEmployeeRequest(
            FirstName: "Maria",
            LastName: "Santos",
            Email: "maria.santos@empresa.com",
            DocumentNumber: "98765432100",
            Password: "Senha@123",
            BirthDate: DateTime.Today.AddYears(-25),
            Role: Role.Director,
            ManagerId: null,
            Phones: new List<PhoneDto> { new(null, "11999999999", "Celular") }
        );

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        // Act
        var act = async () => await _sut.ExecuteAsync(request, creatorId);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*permissão*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var creator = CreateValidEmployee(creatorId, Role.Director);

        var request = new CreateEmployeeRequest(
            FirstName: "Maria",
            LastName: "Santos",
            Email: "maria.santos@empresa.com",
            DocumentNumber: "98765432100",
            Password: "Senha@123",
            BirthDate: DateTime.Today.AddYears(-25),
            Role: Role.Employee,
            ManagerId: null,
            Phones: new List<PhoneDto> { new(null, "11999999999", "Celular") }
        );

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        _employeeRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.ExecuteAsync(request, creatorId);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*e-mail*já está cadastrado*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenDocumentAlreadyExists_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var creator = CreateValidEmployee(creatorId, Role.Director);

        var request = new CreateEmployeeRequest(
            FirstName: "Maria",
            LastName: "Santos",
            Email: "maria.santos@empresa.com",
            DocumentNumber: "98765432100",
            Password: "Senha@123",
            BirthDate: DateTime.Today.AddYears(-25),
            Role: Role.Employee,
            ManagerId: null,
            Phones: new List<PhoneDto> { new(null, "11999999999", "Celular") }
        );

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        _employeeRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.ExistsByDocumentNumberAsync(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.ExecuteAsync(request, creatorId);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*documento*já está cadastrado*");
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

    private static Employee CreateEmployeeFromRequest(Guid id, CreateEmployeeRequest request)
    {
        var employee = new Employee(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            documentNumber: request.DocumentNumber,
            passwordHash: "hashedpassword",
            birthDate: request.BirthDate,
            role: request.Role
        );

        typeof(Entity).GetProperty("Id")!.SetValue(employee, id);

        foreach (var phone in request.Phones)
        {
            employee.AddPhone(new Phone(phone.Number, phone.Type));
        }

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
