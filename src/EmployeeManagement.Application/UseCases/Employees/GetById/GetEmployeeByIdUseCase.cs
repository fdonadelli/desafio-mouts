using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Employees.GetById;

public class GetEmployeeByIdUseCase : IGetEmployeeByIdUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeByIdUseCase> _logger;

    public GetEmployeeByIdUseCase(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        ILogger<GetEmployeeByIdUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando funcionário com ID: {EmployeeId}", id);

        var employee = await _employeeRepository.GetByIdWithPhonesAsync(id, cancellationToken);

        if (employee is null)
        {
            _logger.LogWarning("Funcionário não encontrado: {EmployeeId}", id);
            throw new NotFoundException("Funcionário", id);
        }

        return _mapper.Map<EmployeeResponse>(employee);
    }
}
