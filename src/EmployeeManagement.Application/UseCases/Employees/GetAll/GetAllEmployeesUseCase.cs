using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.GetAll;

public class GetAllEmployeesUseCase : IGetAllEmployeesUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllEmployeesUseCase> _logger;

    public GetAllEmployeesUseCase(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        ILogger<GetAllEmployeesUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listando todos os funcionários");

        var employees = await _employeeRepository.GetAllWithPhonesAsync(cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }
}
