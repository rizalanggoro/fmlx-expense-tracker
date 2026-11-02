using ExpenseTracker.Api.DTOs.Wallets;
using FluentValidation;

namespace ExpenseTracker.Api.Validators.Wallets;

public class CreateWalletValidator : AbstractValidator<CreateWalletDto>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Wallet name is required.")
            .MaximumLength(100).WithMessage("Wallet name cannot exceed 100 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO currency code (e.g., IDR, USD).");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance cannot be negative.");

        RuleFor(x => x.Description)
            .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");
    }
}

