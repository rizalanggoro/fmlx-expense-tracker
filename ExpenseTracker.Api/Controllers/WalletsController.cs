using System.Security.Claims;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController(
    IWalletService walletService,
    IValidator<CreateWalletDto> createValidator,
    IValidator<UpdateWalletDto> updateValidator
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WalletResponseDto>> CreateWallet([FromBody] CreateWalletDto request)
    {
        var validation = await createValidator.ValidateAsync(request);
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await walletService.CreateWalletAsync(userId, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to create wallet",
                    result.Errors
                )
            );

        return CreatedAtAction(
            nameof(GetWalletById),
            new { id = result.Data!.Id },
            result.Data
        );
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WalletResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<WalletResponseDto>>> GetWallets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await walletService.GetWalletsAsync(userId);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to retrieve wallets",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> GetWalletById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await walletService.GetWalletAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Wallet not found",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> UpdateWallet(Guid id, [FromBody] UpdateWalletDto request)
    {
        var validation = await updateValidator.ValidateAsync(request);
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await walletService.UpdateWalletAsync(userId, id, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to update wallet",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await walletService.DeleteWalletAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to delete wallet",
                    result.Errors
                )
            );

        return NoContent();
    }
}