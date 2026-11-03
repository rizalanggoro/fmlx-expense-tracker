using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Api.Services.Implementations;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IMapper mapper
) : IAuthService
{
    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            return ServiceResult<AuthResponseDto>.Failure(
                "An account with this email address already exists.",
                statusCode: 409
            );

        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            return ServiceResult<AuthResponseDto>.Failure(
                "An account with this username already exists.",
                statusCode: 409
            );

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<AuthResponseDto>.Failure(
                "Registration failed.",
                400,
                errors
            );
        }

        var token = tokenService.GenerateToken(user);
        var userDto = mapper.Map<UserResponseDto>(user);

        var response = new AuthResponseDto
        {
            Token = token,
            User = userDto
        };

        return ServiceResult<AuthResponseDto>.Success(
            response,
            "User registered successfully.",
            201
        );
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        ApplicationUser? user;

        if (request.EmailOrUserName.Contains('@'))
            user = await userManager.FindByEmailAsync(request.EmailOrUserName);
        else
            user = await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null)
            return ServiceResult<AuthResponseDto>.Failure(
                "Invalid email/username or password.",
                statusCode: 401
            );

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return ServiceResult<AuthResponseDto>.Failure(
                "Invalid email/username or password.",
                statusCode: 401
            );

        var token = tokenService.GenerateToken(user);
        var userDto = mapper.Map<UserResponseDto>(user);

        var response = new AuthResponseDto
        {
            Token = token,
            User = userDto
        };

        return ServiceResult<AuthResponseDto>.Success(
            response,
            "Login successful."
        );
    }

    public async Task<ServiceResult<UserResponseDto>> GetCurrentUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return ServiceResult<UserResponseDto>.Failure("User not found.", 404);

        var userDto = mapper.Map<UserResponseDto>(user);

        return ServiceResult<UserResponseDto>.Success(userDto);
    }
}