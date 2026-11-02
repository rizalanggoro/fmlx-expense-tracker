using ExpenseTracker.Api.DTOs.Categories;
using FluentValidation;

namespace ExpenseTracker.Api.Validators.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type must be either 1 (Income) or 2 (Expense).");
    }
}
