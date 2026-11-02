using AutoMapper;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Domain.Enums;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.DTOs.Wallets;

namespace ExpenseTracker.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth / User mappings
        CreateMap<ApplicationUser, UserResponseDto>();

        // Wallet mappings
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

        // Category mappings
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WalletId, opt => opt.Ignore())
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

        // Transaction mappings
        CreateMap<CreateTransactionDto, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateTransactionDto, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<Transaction, TransactionResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(src => src.Category != null ? src.Category.Type : (CategoryType)0));
    }
}

