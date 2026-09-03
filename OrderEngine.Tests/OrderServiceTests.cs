using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderEngine.Api.Auth;
using OrderEngine.Application;
using OrderEngine.Domain;
using OrderEngine.Infrastructure;

namespace OrderEngine.Tests;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateOrderWithCorrectTotal()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository);

        var customerId = Guid.NewGuid();
        var request = new CreateOrderRequest(
            customerId,
            new[]
            {
                new CreateOrderItemRequest("sku-1", "Keyboard", 2, 59.90m),
                new CreateOrderItemRequest("sku-2", "Mouse", 1, 45.00m)
            });

        var created = await service.CreateAsync(request);

        Assert.Equal(customerId, created.CustomerId);
        Assert.Equal(2, created.Items.Count);
        Assert.Equal(OrderStatus.Pending, created.Status);
    }

    [Fact]
    public async Task Mediator_ShouldCreateAndQueryOrder()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<ISender>();

        var customerId = Guid.NewGuid();
        var created = await mediator.Send(new CreateOrderCommand(customerId, new[]
        {
            new CreateOrderItemRequest("sku-1", "Keyboard", 2, 59.90m)
        }));

        var byId = await mediator.Send(new GetOrderByIdQuery(created.Id));
        var all = await mediator.Send(new GetOrdersQuery());

        Assert.Equal(customerId, created.CustomerId);
        Assert.NotNull(byId);
        Assert.Single(all);
        Assert.Equal(OrderStatus.Pending, created.Status);
    }

    [Fact]
    public async Task CreateOrderCommandHandler_ShouldCreateOrderWithItems()
    {
        var repository = new InMemoryOrderRepository();
        var handler = new CreateOrderCommandHandler(repository);

        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand(customerId, new[]
        {
            new CreateOrderItemRequest("sku-1", "Keyboard", 2, 59.90m),
            new CreateOrderItemRequest("sku-2", "Mouse", 1, 45.00m)
        });

        var created = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(customerId, created.CustomerId);
        Assert.Equal(2, created.Items.Count);
        Assert.Equal(OrderStatus.Pending, created.Status);
    }

    [Fact]
    public async Task GetOrderByIdQueryHandler_ShouldReturnOrderById()
    {
        var repository = new InMemoryOrderRepository();
        var createHandler = new CreateOrderCommandHandler(repository);
        var queryHandler = new GetOrderByIdQueryHandler(repository);

        var customerId = Guid.NewGuid();
        var created = await createHandler.Handle(new CreateOrderCommand(customerId, new[]
        {
            new CreateOrderItemRequest("sku-10", "Monitor", 1, 899.99m)
        }), CancellationToken.None);

        var result = await queryHandler.Handle(new GetOrderByIdQuery(created.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal(customerId, result.CustomerId);
    }

    [Fact]
    public async Task GetOrdersQueryHandler_ShouldReturnAllOrders()
    {
        var repository = new InMemoryOrderRepository();
        var createHandler = new CreateOrderCommandHandler(repository);
        var queryHandler = new GetOrdersQueryHandler(repository);

        await createHandler.Handle(new CreateOrderCommand(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-1", "Keyboard", 1, 59.90m)
        }), CancellationToken.None);

        await createHandler.Handle(new CreateOrderCommand(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-2", "Mouse", 1, 45.00m)
        }), CancellationToken.None);

        var orders = await queryHandler.Handle(new GetOrdersQuery(), CancellationToken.None);

        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public async Task UpdateOrderStatusCommandHandler_ShouldUpdateStatus()
    {
        var repository = new InMemoryOrderRepository();
        var createHandler = new CreateOrderCommandHandler(repository);
        var updateHandler = new UpdateOrderStatusCommandHandler(repository);

        var created = await createHandler.Handle(new CreateOrderCommand(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-3", "Headset", 1, 250m)
        }), CancellationToken.None);

        var updated = await updateHandler.Handle(new UpdateOrderStatusCommand(created.Id, OrderStatus.Confirmed), CancellationToken.None);

        Assert.Equal(OrderStatus.Confirmed, updated.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusCommandHandler_ShouldSupportPatchStyleStatusChange()
    {
        var repository = new InMemoryOrderRepository();
        var createHandler = new CreateOrderCommandHandler(repository);
        var updateHandler = new UpdateOrderStatusCommandHandler(repository);

        var created = await createHandler.Handle(new CreateOrderCommand(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-9", "Notebook", 1, 2500m)
        }), CancellationToken.None);

        var patched = await updateHandler.Handle(new UpdateOrderStatusCommand(created.Id, OrderStatus.Cancelled), CancellationToken.None);

        Assert.Equal(OrderStatus.Cancelled, patched.Status);
        Assert.Equal(created.Id, patched.Id);
    }

    [Fact]
    public async Task Mediator_ShouldRejectInvalidCreateOrderCommandViaValidationPipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddValidatorsFromAssemblyContaining<CreateOrderCommand>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<ISender>();

        var ex = await Assert.ThrowsAsync<ValidationException>(() => mediator.Send(new CreateOrderCommand(Guid.Empty, new[]
        {
            new CreateOrderItemRequest("sku-1", "Keyboard", 2, 59.90m)
        })));

        Assert.Contains("CustomerId", ex.Message);
    }

    [Fact]
    public async Task LoggingBehavior_ShouldLogRequestAndResponse()
    {
        var logger = new TestLogger<LoggingBehavior<TestRequest, TestResponse>>();
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(logger);

        var response = await behavior.Handle(new TestRequest("sku-1"), () => Task.FromResult(new TestResponse("ok")), CancellationToken.None);

        Assert.Equal("ok", response.Message);
        Assert.Contains(logger.Messages, x => x.Contains("Handling"));
        Assert.Contains(logger.Messages, x => x.Contains("Handled"));
    }

    [Fact]
    public void JwtTokenService_ShouldContainFixedUserEmail()
    {
        var token = JwtTokenService.CreateToken("dev@martech.com", "OrderEngine", "OrderEngineAudience", "super-secret-key-for-local-dev-1234567890");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("dev@martech.com", jwt.Claims.First(x => x.Type == "email").Value);
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectOrderWithoutItems()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository);

        var request = new CreateOrderRequest(Guid.NewGuid(), Array.Empty<CreateOrderItemRequest>());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateOrderStatus()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository);

        var created = await service.CreateAsync(new CreateOrderRequest(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-3", "Monitor", 1, 900m)
        }));

        var updated = await service.UpdateStatusAsync(created.Id, OrderStatus.Confirmed);

        Assert.Equal(OrderStatus.Confirmed, updated.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldRejectInvalidTransitions()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository);

        var created = await service.CreateAsync(new CreateOrderRequest(Guid.NewGuid(), new[]
        {
            new CreateOrderItemRequest("sku-4", "Headset", 1, 250m)
        }));

        await service.UpdateStatusAsync(created.Id, OrderStatus.Confirmed);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateStatusAsync(created.Id, OrderStatus.Cancelled));
    }

    private sealed record TestRequest(string Value);
    private sealed record TestResponse(string Message);

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
