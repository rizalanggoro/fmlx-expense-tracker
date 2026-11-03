using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTracker.Api.DTOs.Auth;

namespace ExpenseTracker.Tests.Infrastructure;

public abstract class BaseIntegrationTest
{
    protected CustomWebApplicationFactory Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;

    [SetUp]
    public virtual void Setup()
    {
        Factory = new CustomWebApplicationFactory();
        Client = Factory.CreateClient();
    }

    [TearDown]
    public virtual void TearDown()
    {
        Client.Dispose();
        Factory.Dispose();
    }

    protected async Task<(HttpClient AuthenticatedClient, AuthResponseDto AuthData)> CreateAuthenticatedClientAsync(
        string? username = null,
        string? email = null,
        string password = "Password123!"
    )
    {
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        username ??= $"user_{uniqueId}";
        email ??= $"user_{uniqueId}@example.com";

        var registerDto = new RegisterRequestDto
        {
            UserName = username,
            Email = email,
            Password = password,
            FullName = $"Test User {uniqueId}"
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/Auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        var authData = (await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>())!;

        var authenticatedClient = Factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authData.Token);

        return (authenticatedClient, authData);
    }
}

