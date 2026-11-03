using System.Net;
using System.Net.Http.Json;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Enums;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Tests.Infrastructure;

namespace ExpenseTracker.Tests.Controllers;

[TestFixture]
public class TransactionsControllerTests : BaseIntegrationTest
{
    private async Task<(HttpClient Client, CategoryResponseDto Category)> SetupUserWithCategoryAsync()
    {
        var (authClient, _) = await CreateAuthenticatedClientAsync();

        var walletResponse = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Main Wallet" });
        var wallet = (await walletResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        var categoryResponse = await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "Food",
            Type = CategoryType.Expense
        });
        var category = (await categoryResponse.Content.ReadFromJsonAsync<CategoryResponseDto>())!;

        return (authClient, category);
    }

    [Test]
    public async Task CreateTransaction_WithValidData_Returns201Created()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();
        var request = new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 50000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Lunch"
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Transactions", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);

        var data = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Amount, Is.EqualTo(50000m));
        Assert.That(data.Description, Is.EqualTo("Lunch"));
        Assert.That(data.CategoryId, Is.EqualTo(category.Id));
    }

    [Test]
    public async Task CreateTransaction_NonExistentCategory_Returns404NotFound()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var request = new CreateTransactionDto
        {
            CategoryId = Guid.NewGuid(), // Non-existent
            Amount = 10000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Ghost category"
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Transactions", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateTransaction_WithZeroOrNegativeAmount_Returns400BadRequest()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();
        var request = new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = -500m, // Invalid amount
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Transactions", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetTransactions_Paged_Returns200OkWithPagedResult()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();

        // Create 2 transactions
        await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 25000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Item 1"
        });
        await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 75000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Item 2"
        });

        // Act
        var response = await authClient.GetAsync("/api/Transactions?pageIndex=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<TransactionResponseDto>>();
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult!.TotalCount, Is.EqualTo(2));
        Assert.That(pagedResult.Items.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetTransactions_FilterByCategoryId_ReturnsMatchingTransactions()
    {
        // Arrange
        var (authClient, category1) = await SetupUserWithCategoryAsync();

        // Create another category
        var walletResponse = await authClient.GetAsync("/api/Wallets");
        var wallets = (await walletResponse.Content.ReadFromJsonAsync<List<WalletResponseDto>>())!;
        var cat2Response = await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallets[0].Id,
            Name = "Transport",
            Type = CategoryType.Expense
        });
        var category2 = (await cat2Response.Content.ReadFromJsonAsync<CategoryResponseDto>())!;

        await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category1.Id,
            Amount = 10000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Food item"
        });
        await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category2.Id,
            Amount = 20000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Bus ticket"
        });

        // Act - Filter by category 2
        var response = await authClient.GetAsync($"/api/Transactions?categoryId={category2.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<TransactionResponseDto>>();
        Assert.That(pagedResult!.Items.All(t => t.CategoryId == category2.Id), Is.True);
    }

    [Test]
    public async Task GetTransactionById_ExistingTransaction_Returns200Ok()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 30000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Coffee"
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<TransactionResponseDto>())!;

        // Act
        var response = await authClient.GetAsync($"/api/Transactions/{created.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
        Assert.That(transaction!.Id, Is.EqualTo(created.Id));
        Assert.That(transaction.Amount, Is.EqualTo(30000m));
    }

    [Test]
    public async Task UpdateTransaction_WithValidData_Returns200Ok()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 30000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Initial"
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<TransactionResponseDto>())!;

        var updateRequest = new UpdateTransactionDto
        {
            Amount = 45000m,
            TransactionDate = DateTime.UtcNow,
            Description = "Updated description"
        };

        // Act
        var response = await authClient.PutAsJsonAsync($"/api/Transactions/{created.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var updated = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
        Assert.That(updated!.Amount, Is.EqualTo(45000m));
        Assert.That(updated.Description, Is.EqualTo("Updated description"));
    }

    [Test]
    public async Task DeleteTransaction_ExistingTransaction_Returns204NoContent()
    {
        // Arrange
        var (authClient, category) = await SetupUserWithCategoryAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Transactions", new CreateTransactionDto
        {
            CategoryId = category.Id,
            Amount = 15000m,
            TransactionDate = DateTime.UtcNow
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<TransactionResponseDto>())!;

        // Act
        var deleteResponse = await authClient.DeleteAsync($"/api/Transactions/{created.Id}");

        // Assert
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify deleted
        var getResponse = await authClient.GetAsync($"/api/Transactions/{created.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
