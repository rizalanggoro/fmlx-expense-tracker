using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
public class TransactionsController : ApiControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("~/api/v1/categories/{categoryId:guid}/transactions")]
    public async Task<IActionResult> GetTransactionsByCategory(
        Guid categoryId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _transactionService.GetPagedTransactionsByCategoryIdAsync(
            categoryId, CurrentUserId, pageIndex, pageSize, startDate, endDate);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _transactionService.GetTransactionByIdAsync(id, CurrentUserId);
        return HandleResult(result);
    }

    [HttpPost("~/api/v1/categories/{categoryId:guid}/transactions")]
    public async Task<IActionResult> CreateTransaction(Guid categoryId, [FromBody] CreateTransactionDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _transactionService.CreateTransactionAsync(categoryId, CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _transactionService.UpdateTransactionAsync(id, CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _transactionService.DeleteTransactionAsync(id, CurrentUserId);
        return HandleResult(result);
    }
}
