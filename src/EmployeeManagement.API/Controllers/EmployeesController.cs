using EmployeeManagement.API.Extensions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.UseCases.Employees.ChangePassword;
using EmployeeManagement.Application.UseCases.Employees.Create;
using EmployeeManagement.Application.UseCases.Employees.Delete;
using EmployeeManagement.Application.UseCases.Employees.GetAll;
using EmployeeManagement.Application.UseCases.Employees.GetById;
using EmployeeManagement.Application.UseCases.Employees.GetSubordinates;
using EmployeeManagement.Application.UseCases.Employees.Update;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

/// <summary>
/// Controller de funcionários.
/// Implementa operações CRUD para gerenciamento de funcionários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IGetAllEmployeesUseCase _getAllEmployeesUseCase;
    private readonly IGetEmployeeByIdUseCase _getEmployeeByIdUseCase;
    private readonly IGetSubordinatesUseCase _getSubordinatesUseCase;
    private readonly ICreateEmployeeUseCase _createEmployeeUseCase;
    private readonly IUpdateEmployeeUseCase _updateEmployeeUseCase;
    private readonly IDeleteEmployeeUseCase _deleteEmployeeUseCase;
    private readonly IChangePasswordUseCase _changePasswordUseCase;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;
    private readonly IValidator<UpdateEmployeeRequest> _updateValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;

    public EmployeesController(
        IGetAllEmployeesUseCase getAllEmployeesUseCase,
        IGetEmployeeByIdUseCase getEmployeeByIdUseCase,
        IGetSubordinatesUseCase getSubordinatesUseCase,
        ICreateEmployeeUseCase createEmployeeUseCase,
        IUpdateEmployeeUseCase updateEmployeeUseCase,
        IDeleteEmployeeUseCase deleteEmployeeUseCase,
        IChangePasswordUseCase changePasswordUseCase,
        IValidator<CreateEmployeeRequest> createValidator,
        IValidator<UpdateEmployeeRequest> updateValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator)
    {
        _getAllEmployeesUseCase = getAllEmployeesUseCase;
        _getEmployeeByIdUseCase = getEmployeeByIdUseCase;
        _getSubordinatesUseCase = getSubordinatesUseCase;
        _createEmployeeUseCase = createEmployeeUseCase;
        _updateEmployeeUseCase = updateEmployeeUseCase;
        _deleteEmployeeUseCase = deleteEmployeeUseCase;
        _changePasswordUseCase = changePasswordUseCase;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    /// <summary>
    /// Lista todos os funcionários ativos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de funcionários</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _getAllEmployeesUseCase.ExecuteAsync(cancellationToken);
        return Ok(employees);
    }

    /// <summary>
    /// Obtém um funcionário pelo ID.
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do funcionário</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _getEmployeeByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(employee);
    }

    /// <summary>
    /// Lista os subordinados de um gerente.
    /// </summary>
    /// <param name="managerId">ID do gerente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de subordinados</returns>
    [HttpGet("manager/{managerId:guid}/subordinates")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetSubordinates(
        Guid managerId,
        CancellationToken cancellationToken)
    {
        var employees = await _getSubordinatesUseCase.ExecuteAsync(managerId, cancellationToken);
        return Ok(employees);
    }

    /// <summary>
    /// Cria um novo funcionário.
    /// </summary>
    /// <param name="request">Dados do funcionário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Funcionário criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var creatorId = User.GetUserId();
        var employee = await _createEmployeeUseCase.ExecuteAsync(request, creatorId, cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Atualiza um funcionário existente.
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="request">Dados atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Funcionário atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var updaterId = User.GetUserId();
        var employee = await _updateEmployeeUseCase.ExecuteAsync(id, request, updaterId, cancellationToken);
        
        return Ok(employee);
    }

    /// <summary>
    /// Exclui (desativa) um funcionário.
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>NoContent</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _deleteEmployeeUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Altera a senha do funcionário autenticado.
    /// </summary>
    /// <param name="request">Dados da alteração de senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>NoContent</returns>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var userId = User.GetUserId();
        await _changePasswordUseCase.ExecuteAsync(userId, request, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Obtém o perfil do funcionário autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do funcionário</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmployeeResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var employee = await _getEmployeeByIdUseCase.ExecuteAsync(userId, cancellationToken);
        return Ok(employee);
    }
}
