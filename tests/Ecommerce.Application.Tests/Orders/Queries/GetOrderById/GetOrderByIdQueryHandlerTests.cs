using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Orders.Queries.GetOrderById;
using Ecommerce.Domain.Entities;
using NSubstitute;

namespace Ecommerce.Application.Tests.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            new[]
            {
                new Order.OrderItemInput(
                    "Keyboard",
                    2,
                    100m)
            });

        var repository = Substitute.For<IOrderRepository>();

        repository
            .GetByIdAsync(
                order.Id,
                Arg.Any<CancellationToken>())
            .Returns(order);

        var handler =
            new GetOrderByIdQueryHandler(repository);

        var query = new GetOrderByIdQuery(order.Id);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(200m, result.TotalAmount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
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
            new GetOrderByIdQueryHandler(repository);

        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}