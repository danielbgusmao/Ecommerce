using Ecommerce.Application.Abstractions.Persistence;
using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, PagedOrdersResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedOrdersResponse> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) =
            await _orderRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

        var items = orders
            .Select(order => new OrderListItemResponse(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.CreatedAt,
                order.TotalAmount))
            .ToList();

        return new PagedOrdersResponse(
            request.Page,
            request.PageSize,
            totalCount,
            items);
    }
}