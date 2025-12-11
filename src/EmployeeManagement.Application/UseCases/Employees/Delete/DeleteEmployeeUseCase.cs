using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.Delete;

public class DeleteEmployeeUseCase : IDeleteEmployeeUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEmployeeUseCase> _logger;

    public DeleteEmployeeUseCase(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteEmployeeUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Excluindo funcionário: {EmployeeId}", id);

        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new NotFoundException("Funcionário", id);
        }

        employee.Deactivate();
        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Funcionário desativado com sucesso: {EmployeeId}", id);
    }
}

