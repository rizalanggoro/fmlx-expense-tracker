using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("~/api/v1/wallets/{walletId:guid}/categories")]
    public async Task<IActionResult> GetCategoriesByWallet(Guid walletId)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _categoryService.GetCategoriesByWalletIdAsync(walletId, CurrentUserId);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _categoryService.GetCategoryByIdAsync(id, CurrentUserId);
        return HandleResult(result);
    }

    [HttpPost("~/api/v1/wallets/{walletId:guid}/categories")]
    public async Task<IActionResult> CreateCategory(Guid walletId, [FromBody] CreateCategoryDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _categoryService.CreateCategoryAsync(walletId, CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _categoryService.UpdateCategoryAsync(id, CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _categoryService.DeleteCategoryAsync(id, CurrentUserId);
        return HandleResult(result);
    }
}
