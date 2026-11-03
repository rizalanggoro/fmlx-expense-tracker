using System.Security.Claims;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController(
    ICategoryService categoryService,
    IValidator<CreateCategoryDto> createValidator,
    IValidator<UpdateCategoryDto> updateValidator
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto request)
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

        var result = await categoryService.CreateAsync(userId, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to create category",
                    result.Errors
                )
            );

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = result.Data!.Id },
            result.Data
        );
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories([FromQuery] Guid? walletId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await categoryService.GetAllAsync(userId, walletId);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to retrieve categories",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await categoryService.GetAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Category not found",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto request)
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

        var result = await categoryService.UpdateAsync(userId, id, request);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to update category",
                    result.Errors
                )
            );

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await categoryService.DeleteAsync(userId, id);
        if (!result.IsSuccess)
            return StatusCode(
                result.StatusCode,
                new ErrorResponse(
                    result.Message ?? "Failed to delete category",
                    result.Errors
                )
            );

        return NoContent();
    }
}