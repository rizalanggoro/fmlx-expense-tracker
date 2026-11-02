using System.Security.Claims;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WalletsController(
    IWalletService walletService,
    IValidator<CreateWalletDto> createValidator,
    IValidator<UpdateWalletDto> updateValidator
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWallets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await walletService.GetWalletsAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWalletById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await walletService.GetWalletByIdAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto request)
    {
        var validation = await createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ServiceResult<object>.Failure("Validation failed", errors, 400));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await walletService.CreateWalletAsync(userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWallet(Guid id, [FromBody] UpdateWalletDto request)
    {
        var validation = await updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ServiceResult<object>.Failure("Validation failed", errors, 400));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await walletService.UpdateWalletAsync(id, userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await walletService.DeleteWalletAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }
}
