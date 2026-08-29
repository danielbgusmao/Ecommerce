namespace Ecommerce.Domain.Entities;

// Private setters protect the entity state from uncontrolled external changes.
// Business invariants are enforced through constructors and domain methods.
public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
        ProductName = string.Empty;
    }

    public OrderItem(
        Guid orderId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order id is required.", nameof(orderId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");

        if (unitPrice <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price must be greater than zero.");

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}