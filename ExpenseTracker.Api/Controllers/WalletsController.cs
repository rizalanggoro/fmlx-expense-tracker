using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
public class WalletsController : ApiControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallets()
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _walletService.GetWalletsAsync(CurrentUserId);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWalletById(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _walletService.GetWalletByIdAsync(id, CurrentUserId);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _walletService.CreateWalletAsync(CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWallet(Guid id, [FromBody] UpdateWalletDto request)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _walletService.UpdateWalletAsync(id, CurrentUserId, request);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _walletService.DeleteWalletAsync(id, CurrentUserId);
        return HandleResult(result);
    }
}
