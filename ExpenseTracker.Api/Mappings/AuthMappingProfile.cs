using AutoMapper;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Auth;

namespace ExpenseTracker.Api.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<ApplicationUser, UserResponseDto>();
    }
}

