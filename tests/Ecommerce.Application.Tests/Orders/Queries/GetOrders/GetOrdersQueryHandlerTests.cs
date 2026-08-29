using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Orders.Queries.GetOrders;
using Ecommerce.Domain.Entities;
using NSubstitute;

namespace Ecommerce.Application.Tests.Orders.Queries.GetOrders;

public class GetOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedOrders()
    {
        // Arrange
        var firstOrder = new Order(
            Guid.NewGuid(),
            new[]
            {
                new Order.OrderItemInput(
                    "Keyboard",
                    2,
                    100m)
            });

        var secondOrder = new Order(
            Guid.NewGuid(),
            new[]
            {
                new Order.OrderItemInput(
                    "Mouse",
                    1,
                    50m)
            });

        var repository = Substitute.For<IOrderRepository>();

        repository
            .GetPagedAsync(
                1,
                10,
                Arg.Any<CancellationToken>())
            .Returns((
                new List<Order>
                {
                    firstOrder,
                    secondOrder
                },
                2));

        var handler =
            new GetOrdersQueryHandler(repository);

        var query = new GetOrdersQuery(
            Page: 1,
            PageSize: 10);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);

        Assert.Contains(
            result.Items,
            item =>
                item.Id == firstOrder.Id &&
                item.TotalAmount == 200m);

        Assert.Contains(
            result.Items,
            item =>
                item.Id == secondOrder.Id &&
                item.TotalAmount == 50m);
    }
}