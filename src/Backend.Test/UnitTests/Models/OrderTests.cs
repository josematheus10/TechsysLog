using Backend.Models;
using FluentAssertions;

namespace Backend.Test.UnitTests.Models;

public class OrderTests
{
    [Fact]
    public void Order_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var order = new Order();

        // Assert
        order.Id.Should().NotBeNullOrEmpty();
        order.OrderNumber.Should().BeEmpty();
        order.Description.Should().BeEmpty();
        order.Value.Should().Be(0);
        order.DeliveryAddress.Should().NotBeNull();
        order.Status.Should().Be("novo");
        order.UserId.Should().BeEmpty();
        order.UserName.Should().BeNull();
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        order.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Order_Id_IsGeneratedGuid()
    {
        // Act
        var order1 = new Order();
        var order2 = new Order();

        // Assert
        order1.Id.Should().NotBe(order2.Id);
        Guid.TryParse(order1.Id, out _).Should().BeTrue();
        Guid.TryParse(order2.Id, out _).Should().BeTrue();
    }

    [Fact]
    public void Order_CanSetAllProperties()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;
        var deliveryAddress = new DeliveryAddress
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Value = 150.75m,
            DeliveryAddress = deliveryAddress,
            Status = "em transporte",
            UserId = "user-123",
            UserName = "testuser",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        order.Id.Should().Be(orderId);
        order.OrderNumber.Should().Be("ORD-001");
        order.Description.Should().Be("Test Order");
        order.Value.Should().Be(150.75m);
        order.DeliveryAddress.Should().Be(deliveryAddress);
        order.Status.Should().Be("em transporte");
        order.UserId.Should().Be("user-123");
        order.UserName.Should().Be("testuser");
        order.CreatedAt.Should().Be(createdAt);
        order.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Order_WithoutUserName_AllowsNullValue()
    {
        // Act
        var order = new Order
        {
            OrderNumber = "ORD-002",
            UserId = "user-456",
            UserName = null
        };

        // Assert
        order.UserName.Should().BeNull();
    }
}
