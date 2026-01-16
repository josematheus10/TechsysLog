using Backend.DTOs.Request;
using Backend.Mappers;
using Backend.Models;
using FluentAssertions;

namespace Backend.Test.UnitTests.Mappers;

public class OrderMapperTests
{
    [Fact]
    public void ToModel_WithValidDto_ReturnsOrderModel()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Value = 100.50m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test Neighborhood",
                City = "Test City",
                State = "SP"
            }
        };
        var userId = "user-123";
        var userName = "testuser";

        // Act
        var order = dto.ToModel(userId, userName);

        // Assert
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be(dto.OrderNumber);
        order.Description.Should().Be(dto.Description);
        order.Value.Should().Be(dto.Value);
        order.UserId.Should().Be(userId);
        order.UserName.Should().Be(userName);
        order.DeliveryAddress.Should().NotBeNull();
        order.DeliveryAddress.Cep.Should().Be(dto.DeliveryAddress.Cep);
        order.DeliveryAddress.Street.Should().Be(dto.DeliveryAddress.Street);
        order.DeliveryAddress.Number.Should().Be(dto.DeliveryAddress.Number);
        order.DeliveryAddress.Neighborhood.Should().Be(dto.DeliveryAddress.Neighborhood);
        order.DeliveryAddress.City.Should().Be(dto.DeliveryAddress.City);
        order.DeliveryAddress.State.Should().Be("SP");
        order.Status.Should().Be("novo");
        order.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToModel_WithNullUserName_ReturnsOrderWithNullUserName()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "ORD-002",
            Description = "Test Order",
            Value = 50.00m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test Neighborhood",
                City = "Test City",
                State = "RJ"
            }
        };
        var userId = "user-456";

        // Act
        var order = dto.ToModel(userId, null);

        // Assert
        order.UserName.Should().BeNull();
        order.UserId.Should().Be(userId);
    }

    [Fact]
    public void ToDto_WithValidOrder_ReturnsOrderResponseDto()
    {
        // Arrange
        var order = new Order
        {
            Id = "order-123",
            OrderNumber = "ORD-003",
            Description = "Test Order",
            Value = 200.00m,
            Status = "novo",
            UserId = "user-789",
            UserName = "testuser",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeliveryAddress = new DeliveryAddress
            {
                Cep = "98765-432",
                Street = "Another Street",
                Number = "456",
                Neighborhood = "Another Neighborhood",
                City = "Another City",
                State = "MG"
            }
        };

        // Act
        var dto = order.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(order.Id);
        dto.OrderNumber.Should().Be(order.OrderNumber);
        dto.Description.Should().Be(order.Description);
        dto.Value.Should().Be(order.Value);
        dto.Status.Should().Be(order.Status);
        dto.UserId.Should().Be(order.UserId);
        dto.UserName.Should().Be(order.UserName);
        dto.CreatedAt.Should().Be(order.CreatedAt);
        dto.UpdatedAt.Should().Be(order.UpdatedAt);
        dto.DeliveryAddress.Should().NotBeNull();
        dto.DeliveryAddress.Cep.Should().Be(order.DeliveryAddress.Cep);
        dto.DeliveryAddress.Street.Should().Be(order.DeliveryAddress.Street);
    }

    [Fact]
    public void ToDto_WithOrderCollection_ReturnsOrderResponseDtoCollection()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                Id = "order-1",
                OrderNumber = "ORD-001",
                Description = "First Order",
                Value = 100.00m,
                DeliveryAddress = new DeliveryAddress { Cep = "12345-678", Street = "Street 1", Number = "1", Neighborhood = "N1", City = "City1", State = "SP" }
            },
            new Order
            {
                Id = "order-2",
                OrderNumber = "ORD-002",
                Description = "Second Order",
                Value = 200.00m,
                DeliveryAddress = new DeliveryAddress { Cep = "98765-432", Street = "Street 2", Number = "2", Neighborhood = "N2", City = "City2", State = "RJ" }
            }
        };

        // Act
        var dtos = orders.ToDto().ToList();

        // Assert
        dtos.Should().HaveCount(2);
        dtos[0].OrderNumber.Should().Be("ORD-001");
        dtos[1].OrderNumber.Should().Be("ORD-002");
    }
}
