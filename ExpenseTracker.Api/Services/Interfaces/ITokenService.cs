using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
}
