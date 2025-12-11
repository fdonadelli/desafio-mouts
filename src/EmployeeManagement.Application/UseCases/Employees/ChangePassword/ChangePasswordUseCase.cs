using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordUseCase> _logger;

    public ChangePasswordUseCase(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Alterando senha do funcionário: {EmployeeId}", id);

        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new NotFoundException("Funcionário", id);
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, employee.PasswordHash))
        {
            _logger.LogWarning("Senha atual incorreta para funcionário: {EmployeeId}", id);
            throw new BusinessRuleException("A senha atual está incorreta.");
        }

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        employee.SetPasswordHash(newPasswordHash);

        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Senha alterada com sucesso para funcionário: {EmployeeId}", id);
    }
}

