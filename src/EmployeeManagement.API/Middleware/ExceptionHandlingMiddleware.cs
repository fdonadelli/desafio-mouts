using System.Net;
using System.Text.Json;
using EmployeeManagement.Domain.Exceptions;
using FluentValidation;

namespace EmployeeManagement.API.Middleware;

/// <summary>
/// Middleware global para tratamento de exceções.
/// Centraliza o tratamento de erros e padroniza as respostas de erro da API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Erro de validação.";
                errorResponse.Errors = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogWarning("Erro de validação: {Errors}", string.Join(", ", errorResponse.Errors));
                break;

            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundException.Message;
                _logger.LogWarning("Recurso não encontrado: {Message}", notFoundException.Message);
                break;

            case BusinessRuleException businessException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = businessException.Message;
                _logger.LogWarning("Regra de negócio violada: {Message}", businessException.Message);
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Acesso não autorizado.";
                _logger.LogWarning("Tentativa de acesso não autorizado");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Ocorreu um erro interno. Tente novamente mais tarde.";
                _logger.LogError(exception, "Erro interno não tratado: {Message}", exception.Message);
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

