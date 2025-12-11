using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Mappings;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.GetAll;

public class GetAllEmployeesUseCase : IGetAllEmployeesUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<GetAllEmployeesUseCase> _logger;

    public GetAllEmployeesUseCase(
        IEmployeeRepository employeeRepository,
        ILogger<GetAllEmployeesUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listando todos os funcionários");

        var employees = await _employeeRepository.GetAllWithPhonesAsync(cancellationToken);
        return EmployeeMapper.ToResponseList(employees);
    }
}

