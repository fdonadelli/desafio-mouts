using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.Create;

public class CreateEmployeeUseCase : ICreateEmployeeUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateEmployeeUseCase> _logger;

    public CreateEmployeeUseCase(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<CreateEmployeeUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeResponse> ExecuteAsync(CreateEmployeeRequest request, Guid creatorId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando novo funcionário: {Email}", request.Email);

        await ValidateCreatorPermissions(creatorId, request, cancellationToken);
        await ValidateUniqueConstraints(request, cancellationToken);
        await ValidateManager(request.ManagerId, cancellationToken);

        var employee = CreateEmployee(request);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Funcionário criado com sucesso: {EmployeeId}", employee.Id);

        var createdEmployee = await _employeeRepository.GetByIdWithPhonesAsync(employee.Id, cancellationToken);
        return _mapper.Map<EmployeeResponse>(createdEmployee!);
    }

    private async Task ValidateCreatorPermissions(Guid creatorId, CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var creator = await _employeeRepository.GetByIdAsync(creatorId, cancellationToken);
        if (creator is null)
        {
            throw new NotFoundException("Funcionário criador", creatorId);
        }

        if (!creator.CanCreateEmployeeWithRole(request.Role))
        {
            _logger.LogWarning(
                "Usuário {CreatorId} com cargo {CreatorRole} tentou criar funcionário com cargo {TargetRole}",
                creatorId, creator.Role, request.Role);
            throw new BusinessRuleException(
                $"Você não tem permissão para criar um funcionário com o cargo {request.Role}. " +
                $"Seu cargo atual é {creator.Role}.");
        }
    }

    private async Task ValidateUniqueConstraints(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (await _employeeRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _logger.LogWarning("E-mail já cadastrado: {Email}", request.Email);
            throw new BusinessRuleException($"O e-mail '{request.Email}' já está cadastrado.");
        }

        if (await _employeeRepository.ExistsByDocumentNumberAsync(request.DocumentNumber, cancellationToken))
        {
            _logger.LogWarning("Documento já cadastrado: {DocumentNumber}", request.DocumentNumber);
            throw new BusinessRuleException($"O documento '{request.DocumentNumber}' já está cadastrado.");
        }
    }

    private async Task ValidateManager(Guid? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue) return;

        var manager = await _employeeRepository.GetByIdAsync(managerId.Value, cancellationToken);
        if (manager is null)
        {
            throw new NotFoundException("Gerente", managerId.Value);
        }
    }

    private Employee CreateEmployee(CreateEmployeeRequest request)
    {
        var passwordHash = _passwordHasher.Hash(request.Password);

        var employee = new Employee(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            documentNumber: request.DocumentNumber,
            passwordHash: passwordHash,
            birthDate: request.BirthDate,
            role: request.Role,
            managerId: request.ManagerId
        );

        foreach (var phoneDto in request.Phones)
        {
            var phone = new Phone(phoneDto.Number, phoneDto.Type);
            employee.AddPhone(phone);
        }

        return employee;
    }
}
