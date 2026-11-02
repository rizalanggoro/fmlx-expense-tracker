using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user);
}

