using System.Security.Claims;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController(
    ITransactionService transactionService
) : ControllerBase
{
    [HttpGet("~/api/v1/categories/{categoryId:guid}/transactions")]
    public async Task<IActionResult> GetTransactionsByCategory(
        Guid categoryId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await transactionService.GetPagedTransactionsByCategoryIdAsync(
            categoryId, userId, pageIndex, pageSize, startDate, endDate);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await transactionService.GetTransactionByIdAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("~/api/v1/categories/{categoryId:guid}/transactions")]
    public async Task<IActionResult> CreateTransaction(Guid categoryId, [FromBody] CreateTransactionDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await transactionService.CreateTransactionAsync(categoryId, userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await transactionService.UpdateTransactionAsync(id, userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await transactionService.DeleteTransactionAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }
}