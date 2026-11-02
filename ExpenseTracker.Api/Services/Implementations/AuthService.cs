using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return ServiceResult<AuthResponseDto>.Failure("An account with this email address already exists.", statusCode: 409);
        }

        var existingUser = await _userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
        {
            return ServiceResult<AuthResponseDto>.Failure("An account with this username already exists.", statusCode: 409);
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ServiceResult<AuthResponseDto>.Failure("Registration failed.", errors, statusCode: 400);
        }

        var (token, expiresAtUtc) = _tokenService.GenerateToken(user);
        var userDto = _mapper.Map<UserResponseDto>(user);

        var response = new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = userDto
        };

        return ServiceResult<AuthResponseDto>.Success(response, "User registered successfully.", statusCode: 201);
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        ApplicationUser? user = null;

        if (request.EmailOrUserName.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.EmailOrUserName);
        }

        if (user == null)
        {
            return ServiceResult<AuthResponseDto>.Failure("Invalid email/username or password.", statusCode: 401);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return ServiceResult<AuthResponseDto>.Failure("Invalid email/username or password.", statusCode: 401);
        }

        var (token, expiresAtUtc) = _tokenService.GenerateToken(user);
        var userDto = _mapper.Map<UserResponseDto>(user);

        var response = new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = userDto
        };

        return ServiceResult<AuthResponseDto>.Success(response, "Login successful.");
    }

    public async Task<ServiceResult<UserResponseDto>> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<UserResponseDto>.NotFound("User not found.");
        }

        var userDto = _mapper.Map<UserResponseDto>(user);
        return ServiceResult<UserResponseDto>.Success(userDto);
    }
}
