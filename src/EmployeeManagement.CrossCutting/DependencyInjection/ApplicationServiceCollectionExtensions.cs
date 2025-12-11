using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.UseCases.Auth.Login;
using EmployeeManagement.Application.UseCases.Employees.ChangePassword;
using EmployeeManagement.Application.UseCases.Employees.Create;
using EmployeeManagement.Application.UseCases.Employees.Delete;
using EmployeeManagement.Application.UseCases.Employees.GetAll;
using EmployeeManagement.Application.UseCases.Employees.GetById;
using EmployeeManagement.Application.UseCases.Employees.GetSubordinates;
using EmployeeManagement.Application.UseCases.Employees.Update;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.CrossCutting.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.CrossCutting.DependencyInjection;

/// <summary>
/// Extensões para registro de serviços da camada Application.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Use Cases - Auth
        services.AddScoped<ILoginUseCase, LoginUseCase>();

        // Use Cases - Employees
        services.AddScoped<IGetEmployeeByIdUseCase, GetEmployeeByIdUseCase>();
        services.AddScoped<IGetAllEmployeesUseCase, GetAllEmployeesUseCase>();
        services.AddScoped<IGetSubordinatesUseCase, GetSubordinatesUseCase>();
        services.AddScoped<ICreateEmployeeUseCase, CreateEmployeeUseCase>();
        services.AddScoped<IUpdateEmployeeUseCase, UpdateEmployeeUseCase>();
        services.AddScoped<IDeleteEmployeeUseCase, DeleteEmployeeUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();

        // Application Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();

        // AutoMapper
        services.AddAutoMapper(typeof(EmployeeMappingProfile).Assembly);

        return services;
    }
}
