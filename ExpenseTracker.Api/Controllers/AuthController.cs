using System.Security.Claims;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterRequestDto> registerValidator,
    IValidator<LoginRequestDto> loginValidator
) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var validation = await registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                new ErrorResponse(
                    "Validation failed",
                    errors
                )
            );
        }

        var result = await authService.RegisterAsync(request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Registration failed",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var validation = await loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                new ErrorResponse(
                    "Validation failed",
                    errors
                )
            );
        }

        var result = await authService.LoginAsync(request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Login failed",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await authService.GetCurrentUserAsync(userId);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "User not found",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }
}