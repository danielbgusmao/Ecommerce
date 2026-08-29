namespace Ecommerce.Domain.ValueObjects;

public sealed record OrderItemData(
    string ProductName,
    int Quantity,
    decimal UnitPrice);