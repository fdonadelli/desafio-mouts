using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.UseCases.Auth.Login;

public class LoginUseCase : ILoginUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IMapper mapper,
        ILogger<LoginUseCase> logger)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tentativa de login: {Email}", request.Email);

        var employee = await _employeeRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (employee is null)
        {
            _logger.LogWarning("Usuário não encontrado: {Email}", request.Email);
            throw new BusinessRuleException("E-mail ou senha inválidos.");
        }

        if (!employee.IsActive)
        {
            _logger.LogWarning("Tentativa de login de usuário inativo: {Email}", request.Email);
            throw new BusinessRuleException("Usuário inativo. Entre em contato com o administrador.");
        }

        if (!_passwordHasher.Verify(request.Password, employee.PasswordHash))
        {
            _logger.LogWarning("Senha incorreta para usuário: {Email}", request.Email);
            throw new BusinessRuleException("E-mail ou senha inválidos.");
        }

        var (token, expiresAt) = _jwtService.GenerateToken(employee);

        _logger.LogInformation("Login realizado com sucesso: {Email}", request.Email);

        return new LoginResponse(
            Token: token,
            ExpiresAt: expiresAt,
            Employee: _mapper.Map<EmployeeResponse>(employee)
        );
    }
}
