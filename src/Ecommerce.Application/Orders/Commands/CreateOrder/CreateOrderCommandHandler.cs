using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var items = request.Items
            .Select(item => new Order.OrderItemInput(
                item.ProductName,
                item.Quantity,
                item.UnitPrice))
            .ToList();

        var order = new Order(
            request.CustomerId,
            items);

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        return order.Id;
    }
}