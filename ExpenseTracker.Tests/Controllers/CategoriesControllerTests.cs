using System.Net;
using System.Net.Http.Json;
using ExpenseTracker.Api.Domain.Enums;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Tests.Infrastructure;

namespace ExpenseTracker.Tests.Controllers;

[TestFixture]
public class CategoriesControllerTests : BaseIntegrationTest
{
    private async Task<(HttpClient Client, WalletResponseDto Wallet)> SetupUserWithWalletAsync()
    {
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var walletResponse = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Main Wallet" });
        var wallet = (await walletResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;
        return (authClient, wallet);
    }

    [Test]
    public async Task CreateCategory_WithValidData_Returns201Created()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        var request = new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "Food & Groceries",
            Type = CategoryType.Expense
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Categories", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);

        var data = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Name, Is.EqualTo("Food & Groceries"));
        Assert.That(data.Type, Is.EqualTo(CategoryType.Expense));
    }

    [Test]
    public async Task CreateCategory_NonExistentWallet_Returns404NotFound()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var request = new CreateCategoryDto
        {
            WalletId = Guid.NewGuid(), // Non-existent
            Name = "Salary",
            Type = CategoryType.Income
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Categories", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateCategory_WithEmptyName_Returns400BadRequest()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        var request = new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "", // Invalid
            Type = CategoryType.Expense
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Categories", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetCategories_ReturnsCurrentUserCategories()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "Transport",
            Type = CategoryType.Expense
        });

        // Act
        var response = await authClient.GetAsync("/api/Categories");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponseDto>>();
        Assert.That(categories, Is.Not.Null);
        Assert.That(categories!.Any(c => c.Name == "Transport"), Is.True);
    }

    [Test]
    public async Task GetCategories_FilterByWalletId_ReturnsOnlyMatchingCategories()
    {
        // Arrange
        var (authClient, wallet1) = await SetupUserWithWalletAsync();
        var wallet2Response = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Savings" });
        var wallet2 = (await wallet2Response.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto { WalletId = wallet1.Id, Name = "Cat 1", Type = CategoryType.Expense });
        await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto { WalletId = wallet2.Id, Name = "Cat 2", Type = CategoryType.Income });

        // Act
        var response = await authClient.GetAsync($"/api/Categories?walletId={wallet1.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponseDto>>();
        Assert.That(categories!.All(c => c.WalletId == wallet1.Id), Is.True);
    }

    [Test]
    public async Task GetCategoryById_ExistingCategory_Returns200Ok()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "Entertainment",
            Type = CategoryType.Expense
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<CategoryResponseDto>())!;

        // Act
        var response = await authClient.GetAsync($"/api/Categories/{created.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var category = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        Assert.That(category!.Id, Is.EqualTo(created.Id));
    }

    [Test]
    public async Task UpdateCategory_WithValidData_Returns200Ok()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "Initial Cat",
            Type = CategoryType.Expense
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<CategoryResponseDto>())!;

        var updateRequest = new UpdateCategoryDto
        {
            Name = "Renamed Cat",
            Type = CategoryType.Expense
        };

        // Act
        var response = await authClient.PutAsJsonAsync($"/api/Categories/{created.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var updated = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        Assert.That(updated!.Name, Is.EqualTo("Renamed Cat"));
    }

    [Test]
    public async Task DeleteCategory_ExistingCategory_Returns204NoContent()
    {
        // Arrange
        var (authClient, wallet) = await SetupUserWithWalletAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
        {
            WalletId = wallet.Id,
            Name = "To Delete",
            Type = CategoryType.Expense
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<CategoryResponseDto>())!;

        // Act
        var deleteResponse = await authClient.DeleteAsync($"/api/Categories/{created.Id}");

        // Assert
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify gone
        var getResponse = await authClient.GetAsync($"/api/Categories/{created.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}

