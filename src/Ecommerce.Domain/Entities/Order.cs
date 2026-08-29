using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount =>
        _items.Sum(item => item.UnitPrice * item.Quantity);

    private Order()
    {
    }

    public Order(
        Guid customerId,
        IEnumerable<OrderItemInput> items)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer id is required.",
                nameof(customerId));

        var itemList = items?.ToList()
            ?? throw new ArgumentNullException(nameof(items));

        if (itemList.Count == 0)
            throw new ArgumentException(
                "Order must contain at least one item.",
                nameof(items));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        foreach (var item in itemList)
        {
            AddItem(
                item.ProductName,
                item.Quantity,
                item.UnitPrice);
        }
    }

    public void AddItem(
        string productName,
        int quantity,
        decimal unitPrice)
    {
        var item = new OrderItem(
            Id,
            productName,
            quantity,
            unitPrice);

        _items.Add(item);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }

    public sealed record OrderItemInput(
        string ProductName,
        int Quantity,
        decimal UnitPrice);
}