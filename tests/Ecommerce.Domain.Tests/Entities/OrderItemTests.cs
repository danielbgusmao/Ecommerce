using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Tests.Entities;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldCreateOrderItem_WhenDataIsValid()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var item = new OrderItem(
            orderId,
            "Keyboard",
            2,
            100m);

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(orderId, item.OrderId);
        Assert.Equal("Keyboard", item.ProductName);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(100m, item.UnitPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenQuantityIsNotGreaterThanZero(
        int quantity)
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var act = () => new OrderItem(
            orderId,
            "Keyboard",
            quantity,
            100m);

        // Assert
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Contains(
            "Quantity must be greater than zero",
            exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenUnitPriceIsNotGreaterThanZero(
        decimal unitPrice)
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var act = () => new OrderItem(
            orderId,
            "Keyboard",
            1,
            unitPrice);

        // Assert
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Contains(
            "Unit price must be greater than zero",
            exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenProductNameIsInvalid(
        string productName)
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var act = () => new OrderItem(
            orderId,
            productName,
            1,
            100m);

        // Assert
        var exception =
            Assert.Throws<ArgumentException>(act);

        Assert.Contains(
            "Product name is required",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOrderIdIsEmpty()
    {
        // Act
        var act = () => new OrderItem(
            Guid.Empty,
            "Keyboard",
            1,
            100m);

        // Assert
        var exception =
            Assert.Throws<ArgumentException>(act);

        Assert.Contains(
            "Order id is required",
            exception.Message);
    }
    
}