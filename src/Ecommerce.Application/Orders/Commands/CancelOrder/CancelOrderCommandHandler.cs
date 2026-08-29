using Ecommerce.Application.Abstractions.Persistence;
using MediatR;

namespace Ecommerce.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler
    : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            return false;

        order.Cancel();

        await _orderRepository.UpdateAsync(
            order,
            cancellationToken);

        return true;
    }
}