using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id)
    : IRequest<OrderResponse?>;