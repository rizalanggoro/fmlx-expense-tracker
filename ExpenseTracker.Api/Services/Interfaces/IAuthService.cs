using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Auth;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ServiceResult<UserResponseDto>> GetCurrentUserAsync(string userId);
}

