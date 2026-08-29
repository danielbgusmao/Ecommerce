namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed record PagedOrdersResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyCollection<OrderListItemResponse> Items);

public sealed record OrderListItemResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount);