using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Tests.Infrastructure;

namespace ExpenseTracker.Tests.Controllers;

[TestFixture]
public class AuthControllerTests : BaseIntegrationTest
{
    [Test]
    public async Task Register_WithValidData_Returns200OkWithToken()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            UserName = "johndoe",
            Email = "john@example.com",
            Password = "Password123!",
            FullName = "John Doe"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Auth/register", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Token, Is.Not.Null.And.Not.Empty);
        Assert.That(data.User.Email, Is.EqualTo(request.Email));
        Assert.That(data.User.UserName, Is.EqualTo(request.UserName));
    }

    [Test]
    public async Task Register_WithDuplicateEmail_Returns409Conflict()
    {
        // Arrange
        var first = new RegisterRequestDto
        {
            UserName = "user1",
            Email = "duplicate@example.com",
            Password = "Password123!",
            FullName = "User One"
        };
        await Client.PostAsJsonAsync("/api/Auth/register", first);

        var second = new RegisterRequestDto
        {
            UserName = "user2",
            Email = "duplicate@example.com",
            Password = "Password123!",
            FullName = "User Two"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Auth/register", second);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Register_WithInvalidData_Returns400BadRequest()
    {
        // Arrange - missing email and weak password
        var request = new RegisterRequestDto
        {
            UserName = "",
            Email = "not-an-email",
            Password = "123",
            FullName = null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Auth/register", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_WithValidCredentials_Returns200OkWithToken()
    {
        // Arrange - Register first
        var register = new RegisterRequestDto
        {
            UserName = "loginuser",
            Email = "login@example.com",
            Password = "Password123!",
            FullName = "Login User"
        };
        await Client.PostAsJsonAsync("/api/Auth/register", register);

        var login = new LoginRequestDto
        {
            EmailOrUserName = "login@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Auth/login", login);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Login_WithWrongPassword_Returns401Unauthorized()
    {
        // Arrange
        var register = new RegisterRequestDto
        {
            UserName = "wrongpass",
            Email = "wrongpass@example.com",
            Password = "Password123!",
            FullName = "Wrong Pass"
        };
        await Client.PostAsJsonAsync("/api/Auth/register", register);

        var login = new LoginRequestDto
        {
            EmailOrUserName = "wrongpass@example.com",
            Password = "WrongPassword999!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Auth/login", login);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetCurrentUser_WhenAuthenticated_Returns200OkWithUserInfo()
    {
        // Arrange
        var (authClient, authData) = await CreateAuthenticatedClientAsync();

        // Act
        var response = await authClient.GetAsync("/api/Auth/me");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Id, Is.EqualTo(authData.User.Id));
    }

    [Test]
    public async Task GetCurrentUser_WhenUnauthenticated_Returns401Unauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/Auth/me");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}

