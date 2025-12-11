using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.GetSubordinates;

public class GetSubordinatesUseCase : IGetSubordinatesUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSubordinatesUseCase> _logger;

    public GetSubordinatesUseCase(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        ILogger<GetSubordinatesUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeResponse>> ExecuteAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listando subordinados do gerente: {ManagerId}", managerId);

        var employees = await _employeeRepository.GetByManagerIdAsync(managerId, cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }
}
