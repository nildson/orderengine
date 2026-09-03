using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderEngine.Application;
using OrderEngine.Domain;
using OrderEngine.Infrastructure;

namespace OrderEngine.Tests;

public sealed class OrderApiIntegrationTests
{
    static OrderApiIntegrationTests()
    {
        Environment.SetEnvironmentVariable("ORDERENGINE_AUTH_EMAIL", "dev@martech.com");
        Environment.SetEnvironmentVariable("ORDERENGINE_AUTH_PASSWORD", "Senha@123");
        Environment.SetEnvironmentVariable("ORDERENGINE_JWT_KEY", "local-dev-jwt-key-very-long-secret-value-2026");
        Environment.SetEnvironmentVariable("ORDERENGINE_JWT_ISSUER", "OrderEngine");
        Environment.SetEnvironmentVariable("ORDERENGINE_JWT_AUDIENCE", "OrderEngineAudience");
    }

    [Fact]
    public async Task GetOrders_ShouldReturnSeededOrders_WhenOrdersExist()
    {
        // Arrange
        await using var factory = new OrderApiFactory();
        var order = new Order(Guid.NewGuid());
        order.AddItem("sku-1", "Keyboard", 2, 59.90m);
        await factory.Repository.AddAsync(order);

        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/orders");

        // Assert
        response.EnsureSuccessStatusCode();
        var returned = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(returned);
        Assert.Contains(returned!, x => x.CustomerId == order.CustomerId);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        await using var factory = new OrderApiFactory();
        var client = factory.CreateClient();
        var missingId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/orders/{missingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_ShouldCancelPendingOrder_WithoutRequestBody()
    {
        await using var factory = new OrderApiFactory();
        var order = new Order(Guid.NewGuid());
        order.AddItem("sku-1", "Keyboard", 1, 59.90m);
        await factory.Repository.AddAsync(order);

        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/orders/{order.Id}/cancel");

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        var returned = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(returned);
        Assert.Equal("Cancelled", returned!.Status);
    }

    private sealed class OrderApiFactory : WebApplicationFactory<Program>
    {
        public InMemoryOrderRepository Repository { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IOrderRepository>();
                services.AddSingleton<IOrderRepository>(Repository);

                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                });

                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                        .RequireAuthenticatedUser()
                        .Build();
                });
            });
        }
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, "integration-user"),
                    new Claim(ClaimTypes.Email, "integration@test.com")
                },
                "Test");

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private sealed class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
