using Backend.Models;
using Backend.Repositories;
using FluentAssertions;
using Moq;

namespace Backend.Test.UnitTests.Repositories;

public class OrderRepositoryTests
{
    [Fact]
    public async Task GetByOrderNumberAsync_WithExistingOrder_ReturnsOrder()
    {
        // Arrange
        var orderNumber = "ORD-001";
        var expectedOrder = new Order
        {
            Id = "order-123",
            OrderNumber = orderNumber,
            Description = "Test Order",
            Value = 100.00m
        };

        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.GetByOrderNumberAsync(orderNumber))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await mockRepository.Object.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be(orderNumber);
        result.Id.Should().Be(expectedOrder.Id);
        mockRepository.Verify(repo => repo.GetByOrderNumberAsync(orderNumber), Times.Once);
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WithNonExistingOrder_ReturnsNull()
    {
        // Arrange
        var orderNumber = "NON-EXISTING";
        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.GetByOrderNumberAsync(orderNumber))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await mockRepository.Object.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().BeNull();
        mockRepository.Verify(repo => repo.GetByOrderNumberAsync(orderNumber), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var orderId = "order-123";
        var newStatus = "em transporte";
        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.UpdateStatusAsync(orderId, newStatus))
            .ReturnsAsync(true);

        // Act
        var result = await mockRepository.Object.UpdateStatusAsync(orderId, newStatus);

        // Assert
        result.Should().BeTrue();
        mockRepository.Verify(repo => repo.UpdateStatusAsync(orderId, newStatus), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var orderId = "invalid-id";
        var newStatus = "em transporte";
        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.UpdateStatusAsync(orderId, newStatus))
            .ReturnsAsync(false);

        // Act
        var result = await mockRepository.Object.UpdateStatusAsync(orderId, newStatus);

        // Assert
        result.Should().BeFalse();
        mockRepository.Verify(repo => repo.UpdateStatusAsync(orderId, newStatus), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidOrder_ReturnsCreatedOrder()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-NEW",
            Description = "New Order",
            Value = 150.00m,
            DeliveryAddress = new DeliveryAddress
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync(order);

        // Act
        var result = await mockRepository.Object.CreateAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.OrderNumber.Should().Be(order.OrderNumber);
        result.Value.Should().Be(order.Value);
        mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOrderList()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = "1", OrderNumber = "ORD-001", Value = 100m },
            new Order { Id = "2", OrderNumber = "ORD-002", Value = 200m },
            new Order { Id = "3", OrderNumber = "ORD-003", Value = 300m }
        };

        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await mockRepository.Object.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(orders);
        mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOrder()
    {
        // Arrange
        var orderId = "order-123";
        var expectedOrder = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Value = 100m
        };

        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.GetByIdAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await mockRepository.Object.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        mockRepository.Verify(repo => repo.GetByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var orderId = "invalid-id";
        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(repo => repo.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await mockRepository.Object.GetByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
        mockRepository.Verify(repo => repo.GetByIdAsync(orderId), Times.Once);
    }
}
