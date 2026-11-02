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
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto request)
    {
        var validation = await createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                ServiceResult<object>.Failure(
                    "Validation failed",
                    errors
                )
            );
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.CreateTransactionAsync(userId, request);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(
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

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.GetTransactionByIdAsync(id, userId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto request)
    {
        var validation = await updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                ServiceResult<object>.Failure(
                    "Validation failed",
                    errors
                )
            );
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.UpdateTransactionAsync(id, userId, request);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await transactionService.DeleteTransactionAsync(id, userId);

        return StatusCode(result.StatusCode, result);
    }
}