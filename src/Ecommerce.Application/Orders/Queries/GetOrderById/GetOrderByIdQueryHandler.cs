using Ecommerce.Application.Abstractions.Persistence;
using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            return null;

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.CreatedAt,
            order.TotalAmount,
            order.Items
                .Select(item => new OrderItemResponse(
                    item.Id,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice))
                .ToList());
    }
}