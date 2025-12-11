using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.CrossCutting.Mappings;

/// <summary>
/// Perfil de mapeamento AutoMapper para Employee.
/// </summary>
public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // Employee -> EmployeeResponse
        CreateMap<Employee, EmployeeResponse>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));

        // Phone -> PhoneDto
        CreateMap<Phone, PhoneDto>();
    }
}

