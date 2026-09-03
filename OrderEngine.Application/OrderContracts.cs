using MediatR;
using OrderEngine.Domain;

namespace OrderEngine.Application;

public record CreateOrderItemRequest(string ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record CreateOrderRequest(Guid CustomerId, IEnumerable<CreateOrderItemRequest> Items);

public record CreateOrderCommand(Guid CustomerId, IEnumerable<CreateOrderItemRequest> Items) : IRequest<Order>;

public record GetOrderByIdQuery(Guid Id) : IRequest<Order?>;

public record GetOrdersQuery : IRequest<IReadOnlyCollection<Order>>;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Order>;

public record UpdateOrderStatusRequest(OrderStatus Status);

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}

public interface IOrderService
{
    Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default);
}

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.", nameof(request));

        var items = request.Items.ToList();
        if (items.Count == 0)
            throw new ArgumentException("An order must contain at least one item.", nameof(request));

        var order = new Order(request.CustomerId);

        foreach (var item in items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        return await _orderRepository.AddAsync(order, cancellationToken);
    }
}

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        => _orderRepository.GetByIdAsync(request.Id, cancellationToken);
}

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<Order>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<IReadOnlyCollection<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        => _orderRepository.GetAllAsync(cancellationToken);
}

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Order>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.Id} was not found.");

        order.UpdateStatus(request.Status);
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return order;
    }
}

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        => new CreateOrderCommandHandler(_orderRepository).Handle(new CreateOrderCommand(request.CustomerId, request.Items), cancellationToken);

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => new GetOrderByIdQueryHandler(_orderRepository).Handle(new GetOrderByIdQuery(id), cancellationToken);

    public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        => new GetOrdersQueryHandler(_orderRepository).Handle(new GetOrdersQuery(), cancellationToken);

    public Task<Order> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default)
        => new UpdateOrderStatusCommandHandler(_orderRepository).Handle(new UpdateOrderStatusCommand(id, status), cancellationToken);
}
