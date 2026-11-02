using AutoMapper;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Wallet, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WalletId, opt => opt.Ignore())
            .ForMember(dest => dest.Wallet, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<Category, CategoryResponseDto>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
                src.Transactions.Sum(t => t.Amount)))
            .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src =>
                src.Transactions.Count));
    }
}
