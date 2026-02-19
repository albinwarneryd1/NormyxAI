using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

namespace Normyx.Tests;

public class TenantIsolationTests : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private WebApplicationFactory<Program> _factory = null!;
    private bool _dockerAvailable = true;

    public async Task InitializeAsync()
    {
        try
        {
            _postgres = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("normyx_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _postgres.StartAsync();
            _factory = new CustomFactory(_postgres.GetConnectionString());
        }
        catch (DockerUnavailableException)
        {
            _dockerAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_dockerAvailable)
        {
            _factory.Dispose();
            if (_postgres is not null)
            {
                await _postgres.DisposeAsync();
            }
        }
    }

    [Fact]
    public async Task TenantA_CannotSee_TenantB_System()
    {
        if (!_dockerAvailable)
        {
            return;
        }

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });

        var tenantAToken = await LoginAsync(client, "NordicFin AB", "admin@nordicfin.example", "ChangeMe123!");

        var tenantBRegistration = await client.PostAsJsonAsync("/auth/register", new
        {
            tenantName = "OtherTenant AB",
            email = "admin@othertenant.example",
            displayName = "Other Admin",
            password = "ChangeMe123!"
        });
        tenantBRegistration.EnsureSuccessStatusCode();

        var tenantBAuth = await tenantBRegistration.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(tenantBAuth);

        using var tenantBRequest = new HttpRequestMessage(HttpMethod.Post, "/aisystems")
        {
            Content = JsonContent.Create(new
            {
                name = "TenantB Secret System",
                description = "Should not leak",
                ownerUserId = (Guid?)null
            })
        };
        tenantBRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tenantBAuth!.AccessToken);
        var tenantBCreateResult = await client.SendAsync(tenantBRequest);
        tenantBCreateResult.EnsureSuccessStatusCode();

        using var tenantAListRequest = new HttpRequestMessage(HttpMethod.Get, "/aisystems");
        tenantAListRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tenantAToken);
        var tenantAListResponse = await client.SendAsync(tenantAListRequest);
        tenantAListResponse.EnsureSuccessStatusCode();

        var payload = await tenantAListResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("TenantB Secret System", payload, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> LoginAsync(HttpClient client, string tenant, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            tenantName = tenant,
            email,
            password
        });
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        return auth!.AccessToken;
    }

    private sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);

    private sealed class CustomFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["UseHttpsRedirection"] = "false",
                    ["ConnectionStrings:DefaultConnection"] = connectionString,
                    ["Jwt:SigningKey"] = "test-signing-key-with-minimum-length-1234567890"
                });
            });
        }
    }
}
