using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Orders.Commands.CreateOrder;
using Ecommerce.Domain.Entities;
using NSubstitute;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Tests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrderAndReturnId_WhenCommandIsValid()
    {
        // Arrange
        var repository = Substitute.For<IOrderRepository>();

        var handler = new CreateOrderCommandHandler(repository);

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new[]
            {
                new CreateOrderItemCommand(
                    "Keyboard",
                    2,
                    100m)
            });

        // Act
        var orderId = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, orderId);

        await repository.Received(1)
            .AddAsync(
                Arg.Is<Order>(order =>
                    order.Id == orderId &&
                    order.Status == OrderStatus.Pending &&
                    order.Items.Count == 1 &&
                    order.TotalAmount == 200m),
                Arg.Any<CancellationToken>());
    }
}