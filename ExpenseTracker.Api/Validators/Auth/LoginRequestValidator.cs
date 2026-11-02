using ExpenseTracker.Api.DTOs.Auth;
using FluentValidation;

namespace ExpenseTracker.Api.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .NotEmpty().WithMessage("Email or Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
