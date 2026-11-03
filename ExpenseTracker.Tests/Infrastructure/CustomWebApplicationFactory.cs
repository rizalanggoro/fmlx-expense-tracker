using ExpenseTracker.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment variables for tests
        Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", "Host=localhost;Database=testdb");
        Environment.SetEnvironmentVariable("JWT_KEY", "ExpenseTrackerSuperSecretSecureKeyForJwtToken2026!");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "ExpenseTrackerApi");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "ExpenseTrackerClient");
        Environment.SetEnvironmentVariable("JWT_DURATION_MINUTES", "1440");

        builder.ConfigureServices(services =>
        {
            // Remove existing PostgreSQL DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register InMemory database for fast and isolated tests
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Ensure schema is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

