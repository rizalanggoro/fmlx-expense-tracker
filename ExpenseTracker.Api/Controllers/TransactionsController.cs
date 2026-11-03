using System.Security.Claims;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(
    ITransactionService transactionService,
    IValidator<CreateTransactionDto> createValidator,
    IValidator<UpdateTransactionDto> updateValidator
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TransactionResponseDto>> CreateTransaction([FromBody] CreateTransactionDto request)
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

        var result = await transactionService.CreateTransactionAsync(userId, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to create transaction",
                    result.Errors
                )
            );

        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = result.Data!.Id },
            result.Data
        );
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<TransactionResponseDto>>> GetTransactions(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null
    )
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.GetPagedTransactionsAsync(
            userId,
            categoryId,
            pageIndex,
            pageSize,
            startDate,
            endDate
        );

        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to retrieve transactions",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> GetTransactionById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.GetTransactionByIdAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Transaction not found",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> UpdateTransaction(Guid id,
        [FromBody] UpdateTransactionDto request)
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

        var result = await transactionService.UpdateTransactionAsync(userId, id, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to update transaction",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.DeleteTransactionAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to delete transaction",
                    result.Errors
                )
            );

        return NoContent();
    }
}