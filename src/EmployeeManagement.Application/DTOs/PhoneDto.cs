namespace EmployeeManagement.Application.DTOs;

public record PhoneDto(
    Guid? Id,
    string Number,
    string? Type
);

