using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.Update;

public class UpdateEmployeeUseCase : IUpdateEmployeeUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateEmployeeUseCase> _logger;

    public UpdateEmployeeUseCase(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateEmployeeUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeResponse> ExecuteAsync(Guid id, UpdateEmployeeRequest request, Guid updaterId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando funcionário: {EmployeeId}", id);

        var employee = await GetEmployeeOrThrow(id, cancellationToken);
        await ValidateUpdaterPermissions(updaterId, request, cancellationToken);
        await ValidateEmailUniqueness(id, request.Email, cancellationToken);
        await ValidateManager(id, request.ManagerId, cancellationToken);

        UpdateEmployeeData(employee, request);

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Funcionário atualizado com sucesso: {EmployeeId}", id);

        var updatedEmployee = await _employeeRepository.GetByIdWithPhonesAsync(id, cancellationToken);
        return _mapper.Map<EmployeeResponse>(updatedEmployee!);
    }

    private async Task<Employee> GetEmployeeOrThrow(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdWithPhonesAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new NotFoundException("Funcionário", id);
        }
        return employee;
    }

    private async Task ValidateUpdaterPermissions(Guid updaterId, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var updater = await _employeeRepository.GetByIdAsync(updaterId, cancellationToken);
        if (updater is null)
        {
            throw new NotFoundException("Funcionário atualizador", updaterId);
        }

        if (!updater.CanCreateEmployeeWithRole(request.Role))
        {
            _logger.LogWarning(
                "Usuário {UpdaterId} com cargo {UpdaterRole} tentou atualizar funcionário para cargo {TargetRole}",
                updaterId, updater.Role, request.Role);
            throw new BusinessRuleException(
                $"Você não tem permissão para definir o cargo {request.Role}. " +
                $"Seu cargo atual é {updater.Role}.");
        }
    }

    private async Task ValidateEmailUniqueness(Guid employeeId, string email, CancellationToken cancellationToken)
    {
        var existingByEmail = await _employeeRepository.GetByEmailAsync(email, cancellationToken);
        if (existingByEmail is not null && existingByEmail.Id != employeeId)
        {
            throw new BusinessRuleException($"O e-mail '{email}' já está cadastrado.");
        }
    }

    private async Task ValidateManager(Guid employeeId, Guid? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue) return;

        if (managerId.Value == employeeId)
        {
            throw new BusinessRuleException("Um funcionário não pode ser seu próprio gerente.");
        }

        var manager = await _employeeRepository.GetByIdAsync(managerId.Value, cancellationToken);
        if (manager is null)
        {
            throw new NotFoundException("Gerente", managerId.Value);
        }
    }

    private static void UpdateEmployeeData(Employee employee, UpdateEmployeeRequest request)
    {
        employee.SetFirstName(request.FirstName);
        employee.SetLastName(request.LastName);
        employee.SetEmail(request.Email);
        employee.SetBirthDate(request.BirthDate);
        employee.SetRole(request.Role);
        employee.SetManager(request.ManagerId);

        employee.ClearPhones();
        foreach (var phoneDto in request.Phones)
        {
            var phone = new Phone(phoneDto.Number, phoneDto.Type);
            employee.AddPhone(phone);
        }
    }
}
