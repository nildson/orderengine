namespace OrderEngine.Domain;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public sealed class OrderItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Total => Quantity * UnitPrice;

    public OrderItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId é obrigatório.", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("ProductName é obrigatório.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantidade deve ser maior que zero.");

        if (unitPrice < 1)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Preço unitário deve ser maior ou igual a um.");

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public ICollection<OrderItem> Items => _items;
    public decimal Total => _items.Sum(item => item.Total);

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId é obrigatório.", nameof(customerId));

        CustomerId = customerId;
    }

    public void AddItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem(productId, productName, quantity, unitPrice));
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            throw new InvalidOperationException("Item não encontrado no pedido.");

        _items.Remove(item);
    }

    public void Confirm()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Não é possível confirmar um pedido cancelado.");

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Somente pedidos pendentes podem ser confirmados.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Não é possível cancelar um pedido confirmado.");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Este pedido já está cancelado.");

        Status = OrderStatus.Cancelled;
    }

    public void UpdateStatus(OrderStatus status)
    {
        switch (status)
        {
            case OrderStatus.Pending:
                if (Status != OrderStatus.Pending)
                    throw new InvalidOperationException("Somente pedidos pendentes podem voltar ao status pendente.");
                return;
            case OrderStatus.Confirmed:
                Confirm();
                return;
            case OrderStatus.Cancelled:
                Cancel();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, "Status de pedido não suportado.");
        }
    }
}
