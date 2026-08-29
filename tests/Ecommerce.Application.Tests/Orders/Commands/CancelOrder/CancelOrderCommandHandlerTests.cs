using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Orders.Commands.CancelOrder;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using NSubstitute;

namespace Ecommerce.Application.Tests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCancelOrder_WhenOrderExistsAndIsPending()
    {
        // Arrange
        var order = CreateOrder();

        var repository = Substitute.For<IOrderRepository>();

        repository
            .GetByIdAsync(
                order.Id,
                Arg.Any<CancellationToken>())
            .Returns(order);

        var handler =
            new CancelOrderCommandHandler(repository);

        var command =
            new CancelOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.Cancelled, order.Status);

        await repository.Received(1)
            .UpdateAsync(
                order,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var repository = Substitute.For<IOrderRepository>();

        repository
            .GetByIdAsync(
                orderId,
                Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var handler =
            new CancelOrderCommandHandler(repository);

        var command =
            new CancelOrderCommand(orderId);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await repository.DidNotReceive()
            .UpdateAsync(
                Arg.Any<Order>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenOrderIsAlreadyCancelled()
    {
        // Arrange
        var order = CreateOrder();
        order.Cancel();

        var repository = Substitute.For<IOrderRepository>();

        repository
            .GetByIdAsync(
                order.Id,
                Arg.Any<CancellationToken>())
            .Returns(order);

        var handler =
            new CancelOrderCommandHandler(repository);

        var command =
            new CancelOrderCommand(order.Id);

        // Act
        var act = () => handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(act);

        Assert.Equal(
            "Only pending orders can be cancelled.",
            exception.Message);

        await repository.DidNotReceive()
            .UpdateAsync(
                Arg.Any<Order>(),
                Arg.Any<CancellationToken>());
    }

    private static Order CreateOrder()
    {
        return new Order(
            Guid.NewGuid(),
            new[]
            {
                new Order.OrderItemInput(
                    "Keyboard",
                    1,
                    100m)
            });
    }
}