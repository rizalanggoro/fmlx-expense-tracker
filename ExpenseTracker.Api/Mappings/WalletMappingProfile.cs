using AutoMapper;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Domain.Enums;
using ExpenseTracker.Api.DTOs.Wallets;

namespace ExpenseTracker.Api.Mappings;

public class WalletMappingProfile : Profile
{
    public WalletMappingProfile()
    {
        CreateMap<CreateWalletDto, Wallet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateWalletDto, Wallet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.InitialBalance, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<Wallet, WalletResponseDto>()
            .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src =>
                src.Categories
                    .Where(c => c.Type == CategoryType.Income)
                    .SelectMany(c => c.Transactions)
                    .Sum(t => t.Amount)))
            .ForMember(dest => dest.TotalExpense, opt => opt.MapFrom(src =>
                src.Categories
                    .Where(c => c.Type == CategoryType.Expense)
                    .SelectMany(c => c.Transactions)
                    .Sum(t => t.Amount)))
            .ForMember(dest => dest.CurrentBalance, opt => opt.MapFrom(src =>
                src.InitialBalance +
                src.Categories.Where(c => c.Type == CategoryType.Income).SelectMany(c => c.Transactions).Sum(t => t.Amount) -
                src.Categories.Where(c => c.Type == CategoryType.Expense).SelectMany(c => c.Transactions).Sum(t => t.Amount)));

        CreateMap<Wallet, WalletDetailResponseDto>()
            .IncludeBase<Wallet, WalletResponseDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));
    }
}

