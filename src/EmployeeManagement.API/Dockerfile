# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/EmployeeManagement.API/EmployeeManagement.API.csproj", "src/EmployeeManagement.API/"]
COPY ["src/EmployeeManagement.Application/EmployeeManagement.Application.csproj", "src/EmployeeManagement.Application/"]
COPY ["src/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj", "src/EmployeeManagement.Domain/"]
COPY ["src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj", "src/EmployeeManagement.Infrastructure/"]

RUN dotnet restore "src/EmployeeManagement.API/EmployeeManagement.API.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/EmployeeManagement.API"
RUN dotnet build "EmployeeManagement.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "EmployeeManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Criar usuário não-root para segurança
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.API.dll"]

