# Employee Management API

API REST para gerenciamento de funcionários desenvolvida em .NET 8.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Padrões de Projeto](#padrões-de-projeto)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Como Executar](#como-executar)
- [Documentação da API](#documentação-da-api)
- [Testes](#testes)
- [Decisões Técnicas](#decisões-técnicas)

## 📖 Sobre o Projeto

Sistema de gerenciamento de funcionários de uma empresa fictícia com as seguintes funcionalidades:

- CRUD completo de funcionários
- Autenticação JWT
- Validação de hierarquia (funcionário não pode criar usuário com cargo superior)
- Validação de maioridade (18+ anos)
- Suporte a múltiplos telefones por funcionário
- Relacionamento de gerente/subordinado

### Regras de Negócio

- **Hierarquia de Cargos**: Employee < Leader < Director
- Um funcionário só pode criar outro com cargo igual ou inferior ao seu
- Documento (CPF) é único
- E-mail é único
- Funcionário deve ser maior de idade (18+)
- Cada funcionário deve ter pelo menos um telefone

## 🏗️ Arquitetura

O projeto segue uma **Clean Architecture simplificada** com 4 camadas:

```
┌─────────────────────────────────────────┐
│              API (Presentation)          │
│  Controllers, Middleware, Configuration  │
├─────────────────────────────────────────┤
│            Application                   │
│   Use Cases, DTOs, Validators, Mappings  │
├─────────────────────────────────────────┤
│              Domain                      │
│    Entities, Enums, Interfaces           │
├─────────────────────────────────────────┤
│           Infrastructure                 │
│   DbContext, Repositories, Migrations    │
└─────────────────────────────────────────┘
```

### Fluxo de Dependências

```
API → Application → Domain ← Infrastructure
```

- **Domain**: Núcleo da aplicação, sem dependências externas
- **Application**: Lógica de aplicação (Use Cases), depende apenas do Domain
- **Infrastructure**: Implementações concretas (EF Core, PostgreSQL)
- **API**: Camada de apresentação (Controllers, Middleware)

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Banco de dados
- **JWT Bearer** - Autenticação
- **FluentValidation** - Validação de dados
- **BCrypt.Net** - Hash de senhas
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Framework de testes
- **Moq** - Mocking para testes
- **FluentAssertions** - Assertions para testes
- **Docker** - Containerização

## 📐 Padrões de Projeto

### Use Case Pattern
Cada operação de negócio é encapsulada em um Use Case dedicado, seguindo o princípio de Single Responsibility.

```csharp
public interface ICreateEmployeeUseCase
{
    Task<EmployeeResponse> ExecuteAsync(CreateEmployeeRequest request, Guid creatorId, CancellationToken cancellationToken = default);
}

public class CreateEmployeeUseCase : ICreateEmployeeUseCase
{
    public async Task<EmployeeResponse> ExecuteAsync(CreateEmployeeRequest request, Guid creatorId, CancellationToken cancellationToken = default)
    {
        // Validações e lógica de criação
    }
}
```

### Repository Pattern
Abstrai a camada de dados, permitindo trocar a implementação do banco sem afetar a lógica de negócio.

```csharp
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    // ...
}
```

### Unit of Work
Garante que todas as operações de uma transação sejam commitadas juntas.

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Rich Domain Model
Entidades com comportamento encapsulado e validações internas.

```csharp
public class Employee : Entity
{
    public bool CanCreateEmployeeWithRole(Role targetRole)
    {
        return Role >= targetRole;
    }
}
```

### DTOs (Data Transfer Objects)
Separação entre modelos de domínio e dados de transferência.

```csharp
public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    // ...
);
```

## 📁 Estrutura do Projeto

```
├── src/
│   ├── EmployeeManagement.API/           # Camada de apresentação
│   │   ├── Controllers/                  # Endpoints da API
│   │   ├── Middleware/                   # Exception handling
│   │   ├── Extensions/                   # Service collection extensions
│   │   ├── Configuration/                # JWT settings
│   │   └── Services/                     # JWT service
│   │
│   ├── EmployeeManagement.Application/   # Camada de aplicação
│   │   ├── UseCases/                     # Casos de uso da aplicação
│   │   │   ├── Auth/                     # Use cases de autenticação
│   │   │   │   └── Login/
│   │   │   │       ├── ILoginUseCase.cs
│   │   │   │       └── LoginUseCase.cs
│   │   │   └── Employees/                # Use cases de funcionários
│   │   │       ├── Create/
│   │   │       ├── Update/
│   │   │       ├── Delete/
│   │   │       ├── GetById/
│   │   │       ├── GetAll/
│   │   │       ├── GetSubordinates/
│   │   │       └── ChangePassword/
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Interfaces/                   # Contratos (IJwtService, IPasswordHasher)
│   │   ├── Services/                     # Serviços auxiliares (PasswordHasher)
│   │   ├── Validators/                   # FluentValidation validators
│   │   └── Mappings/                     # Mapeamento Entity <-> DTO
│   │
│   ├── EmployeeManagement.Domain/        # Camada de domínio
│   │   ├── Entities/                     # Entidades do domínio
│   │   ├── Enums/                        # Enumeradores
│   │   ├── Exceptions/                   # Exceções de domínio
│   │   └── Interfaces/                   # Contratos de repositórios
│   │
│   └── EmployeeManagement.Infrastructure/# Camada de infraestrutura
│       ├── Data/                         # DbContext e configurações
│       │   ├── Configurations/           # Entity configurations
│       │   └── Migrations/               # EF migrations
│       └── Repositories/                 # Implementação dos repositórios
│
├── tests/
│   └── EmployeeManagement.Tests/         # Testes unitários
│       ├── Domain/                       # Testes de entidades
│       └── Application/
│           └── UseCases/                 # Testes de use cases
│               ├── Auth/
│               │   └── LoginUseCaseTests.cs
│               └── Employees/
│                   ├── CreateEmployeeUseCaseTests.cs
│                   ├── DeleteEmployeeUseCaseTests.cs
│                   ├── GetEmployeeByIdUseCaseTests.cs
│                   └── ChangePasswordUseCaseTests.cs
│
├── docker-compose.yml                    # Orquestração de containers
├── .dockerignore                         # Arquivos ignorados no build
└── README.md                             # Este arquivo
```

### Use Cases Disponíveis

| Use Case | Descrição |
|----------|-----------|
| `LoginUseCase` | Autenticação de usuário e geração de token JWT |
| `CreateEmployeeUseCase` | Criação de novo funcionário com validações |
| `UpdateEmployeeUseCase` | Atualização de dados do funcionário |
| `DeleteEmployeeUseCase` | Soft delete (desativação) do funcionário |
| `GetEmployeeByIdUseCase` | Busca funcionário por ID |
| `GetAllEmployeesUseCase` | Lista todos os funcionários ativos |
| `GetSubordinatesUseCase` | Lista subordinados de um gerente |
| `ChangePasswordUseCase` | Alteração de senha do usuário |

## 🚀 Como Executar

### Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento local)

### Com Docker (Recomendado)

```bash
# Clone o repositório
git clone <url-do-repositorio>
cd desafio_seilaoq

# Execute com Docker Compose
docker-compose up -d

# A API estará disponível em http://localhost:8080
```

### Desenvolvimento Local

```bash
# Inicie apenas o PostgreSQL
docker-compose up -d postgres

# Restaure as dependências
dotnet restore

# Execute a aplicação
dotnet run --project src/EmployeeManagement.API

# A API estará disponível em http://localhost:5000
```

### Credenciais Padrão

Ao iniciar, um usuário administrador é criado automaticamente:

- **E-mail**: admin@empresa.com
- **Senha**: Admin@123
- **Cargo**: Director

## 📚 Documentação da API

Acesse o Swagger UI em: `http://localhost:8080` (Docker) ou `http://localhost:5000` (local)

### Endpoints Principais

#### Autenticação
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | /api/auth/login | Realiza login e retorna token JWT |

#### Funcionários
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | /api/employees | Lista todos os funcionários |
| GET | /api/employees/{id} | Obtém funcionário por ID |
| GET | /api/employees/me | Obtém perfil do usuário logado |
| GET | /api/employees/manager/{id}/subordinates | Lista subordinados de um gerente |
| POST | /api/employees | Cria novo funcionário |
| PUT | /api/employees/{id} | Atualiza funcionário |
| DELETE | /api/employees/{id} | Desativa funcionário (soft delete) |
| POST | /api/employees/change-password | Altera senha do usuário logado |

### Exemplo de Requisição - Login

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@empresa.com",
    "password": "Admin@123"
  }'
```

### Exemplo de Requisição - Criar Funcionário

```bash
curl -X POST http://localhost:8080/api/employees \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu-token}" \
  -d '{
    "firstName": "João",
    "lastName": "Silva",
    "email": "joao.silva@empresa.com",
    "documentNumber": "12345678900",
    "password": "Senha@123",
    "birthDate": "1990-01-15",
    "role": 1,
    "managerId": null,
    "phones": [
      { "number": "11999999999", "type": "Celular" }
    ]
  }'
```

### Roles (Cargos)

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Employee | Funcionário comum |
| 2 | Leader | Líder de equipe |
| 3 | Director | Diretor |

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Cobertura de Testes

- **Domain**: Testes de entidades e regras de negócio
- **Application**: Testes de use cases com mocks

## 💡 Decisões Técnicas

### Por que Use Cases em vez de Services?
- **Single Responsibility**: Cada use case tem uma única responsabilidade
- **Código enxuto**: Classes pequenas e focadas (~30-50 linhas)
- **Testabilidade**: Mais fácil de testar isoladamente
- **Manutenibilidade**: Alterações em uma funcionalidade não afetam outras
- **Navegação**: Fácil de encontrar onde está a lógica de cada operação

### Por que Clean Architecture?
- Separação clara de responsabilidades
- Facilita testes unitários
- Independência de frameworks e bibliotecas externas
- Facilita manutenção e evolução do código

### Por que PostgreSQL?
- Banco de dados robusto e open source
- Excelente suporte no Entity Framework Core
- Fácil de containerizar

### Por que BCrypt para senhas?
- Algoritmo seguro com salt automático
- Resistente a ataques de força bruta
- Work factor configurável

### Por que FluentValidation?
- Validações expressivas e legíveis
- Fácil de testar
- Separação de responsabilidades (validação fora dos controllers)

### Por que Soft Delete?
- Preserva histórico de dados
- Permite reativação de funcionários
- Evita problemas com integridade referencial

### Por que não usar AutoMapper?
- Para projetos menores, mapeamento manual é mais explícito
- Evita "magia" que pode dificultar debugging
- Menos uma dependência externa

## 📝 Licença

Este projeto foi desenvolvido como parte de um desafio técnico.

## 👤 Autor

Desenvolvido para processo seletivo.
