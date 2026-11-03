using System.Net;
using System.Net.Http.Json;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Tests.Infrastructure;

namespace ExpenseTracker.Tests.Controllers;

[TestFixture]
public class WalletsControllerTests : BaseIntegrationTest
{
    [Test]
    public async Task CreateWallet_WithValidData_Returns201Created()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var request = new CreateWalletDto
        {
            Name = "BCA Main",
            Currency = "IDR",
            InitialBalance = 5000000m,
            Description = "Primary bank account"
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Wallets", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);

        var data = await response.Content.ReadFromJsonAsync<WalletResponseDto>();
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Name, Is.EqualTo("BCA Main"));
        Assert.That(data.InitialBalance, Is.EqualTo(5000000m));
        Assert.That(data.Currency, Is.EqualTo("IDR"));
    }

    [Test]
    public async Task CreateWallet_WithInvalidName_Returns400BadRequest()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var request = new CreateWalletDto
        {
            Name = "", // Invalid
            Currency = "IDR",
            InitialBalance = 1000m
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/Wallets", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetWallets_ReturnsOnlyCurrentUserWallets()
    {
        // Arrange - User 1 creates a wallet
        var (user1Client, _) = await CreateAuthenticatedClientAsync("user1_wallets", "u1w@example.com");
        await user1Client.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "User1 Wallet", Currency = "IDR" });

        // User 2 creates a wallet
        var (user2Client, _) = await CreateAuthenticatedClientAsync("user2_wallets", "u2w@example.com");
        await user2Client.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "User2 Wallet", Currency = "IDR" });

        // Act - User 1 gets wallets
        var response = await user1Client.GetAsync("/api/Wallets");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var wallets = await response.Content.ReadFromJsonAsync<List<WalletResponseDto>>();
        Assert.That(wallets, Is.Not.Null);
        Assert.That(wallets!.Count, Is.EqualTo(1));
        Assert.That(wallets[0].Name, Is.EqualTo("User1 Wallet"));
    }

    [Test]
    public async Task GetWalletById_ExistingWallet_Returns200Ok()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Mandiri", Currency = "IDR" });
        var created = (await createResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        // Act
        var response = await authClient.GetAsync($"/api/Wallets/{created.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var wallet = await response.Content.ReadFromJsonAsync<WalletDetailResponseDto>();
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet!.Id, Is.EqualTo(created.Id));
    }

    [Test]
    public async Task GetWalletById_OtherUserWallet_Returns404NotFound()
    {
        // Arrange
        var (user1Client, _) = await CreateAuthenticatedClientAsync("user1_iso", "u1iso@example.com");
        var createResponse = await user1Client.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Private Wallet" });
        var created = (await createResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        var (user2Client, _) = await CreateAuthenticatedClientAsync("user2_iso", "u2iso@example.com");

        // Act - User 2 tries to access User 1's wallet
        var response = await user2Client.GetAsync($"/api/Wallets/{created.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task UpdateWallet_WithValidData_Returns200Ok()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "Old Name" });
        var created = (await createResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        var updateRequest = new UpdateWalletDto
        {
            Name = "Updated Name",
            Currency = "USD",
            Description = "Updated description"
        };

        // Act
        var response = await authClient.PutAsJsonAsync($"/api/Wallets/{created.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var updated = await response.Content.ReadFromJsonAsync<WalletResponseDto>();
        Assert.That(updated!.Name, Is.EqualTo("Updated Name"));
        Assert.That(updated.Currency, Is.EqualTo("USD"));
    }

    [Test]
    public async Task DeleteWallet_ExistingWallet_Returns204NoContent()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync();
        var createResponse = await authClient.PostAsJsonAsync("/api/Wallets", new CreateWalletDto { Name = "To Delete" });
        var created = (await createResponse.Content.ReadFromJsonAsync<WalletResponseDto>())!;

        // Act
        var deleteResponse = await authClient.DeleteAsync($"/api/Wallets/{created.Id}");

        // Assert
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify deleted
        var getResponse = await authClient.GetAsync($"/api/Wallets/{created.Id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task AccessWallet_WithoutToken_Returns401Unauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/Wallets");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}

