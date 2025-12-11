using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API;

/// <summary>
/// Classe para seed de dados iniciais.
/// Cria um usuário administrador padrão para primeiro acesso.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Verificar se já existe algum funcionário
        if (await context.Employees.AnyAsync())
        {
            logger.LogInformation("Dados já existentes. Seed ignorado.");
            return;
        }

        logger.LogInformation("Criando usuário administrador padrão...");

        // Criar usuário administrador padrão
        var admin = new Employee(
            firstName: "Admin",
            lastName: "Sistema",
            email: "admin@empresa.com",
            documentNumber: "00000000000",
            passwordHash: passwordHasher.Hash("Admin@123"),
            birthDate: new DateTime(1990, 1, 1),
            role: Role.Director
        );

        admin.AddPhone(new Phone("11999999999", "Celular"));

        await context.Employees.AddAsync(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Usuário administrador criado com sucesso. E-mail: admin@empresa.com, Senha: Admin@123");
    }
}

