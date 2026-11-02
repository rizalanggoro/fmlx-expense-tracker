using System.Security.Claims;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoriesController(
    ICategoryService categoryService
) : ControllerBase
{
    [HttpGet("~/api/v1/wallets/{walletId:guid}/categories")]
    public async Task<IActionResult> GetCategoriesByWallet(Guid walletId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await categoryService.GetCategoriesByWalletIdAsync(walletId, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await categoryService.GetCategoryByIdAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("~/api/v1/wallets/{walletId:guid}/categories")]
    public async Task<IActionResult> CreateCategory(Guid walletId, [FromBody] CreateCategoryDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await categoryService.CreateCategoryAsync(walletId, userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await categoryService.UpdateCategoryAsync(id, userId, request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await categoryService.DeleteCategoryAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }
}