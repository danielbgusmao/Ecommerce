using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreatePendingOrder_WhenDataIsValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var items = new[]
        {
            new Order.OrderItemInput(
                "Keyboard",
                2,
                100m)
        };

        // Act
        var order = new Order(customerId, items);

        // Assert
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Single(order.Items);
        Assert.Equal(200m, order.TotalAmount);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCustomerIdIsEmpty()
    {
        // Arrange
        var items = new[]
        {
            new Order.OrderItemInput(
                "Keyboard",
                1,
                100m)
        };

        // Act
        var act = () => new Order(Guid.Empty, items);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("Customer id is required", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenItemsAreEmpty()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = Array.Empty<Order.OrderItemInput>();

        // Act
        var act = () => new Order(customerId, items);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("at least one item", exception.Message);
    }

    [Fact]
    public void TotalAmount_ShouldReturnSumOfAllItems()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var items = new[]
        {
            new Order.OrderItemInput("Keyboard", 2, 100m),
            new Order.OrderItemInput("Mouse", 3, 50m)
        };

        // Act
        var order = new Order(customerId, items);

        // Assert
        Assert.Equal(350m, order.TotalAmount);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled_WhenOrderIsPending()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var items = new[]
        {
            new Order.OrderItemInput(
                "Keyboard",
                1,
                100m)
        };

        var order = new Order(customerId, items);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenOrderIsNotPending()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var items = new[]
        {
            new Order.OrderItemInput(
                "Keyboard",
                1,
                100m)
        };

        var order = new Order(customerId, items);
        order.Confirm();

        // Act
        var act = () => order.Cancel();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);

        Assert.Contains(
            "Only pending orders can be cancelled",
            exception.Message);
    }

}