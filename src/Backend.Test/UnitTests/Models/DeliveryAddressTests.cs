using Backend.Models;
using FluentAssertions;

namespace Backend.Test.UnitTests.Models;

public class DeliveryAddressTests
{
    [Fact]
    public void DeliveryAddress_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var address = new DeliveryAddress();

        // Assert
        address.Cep.Should().BeEmpty();
        address.Street.Should().BeEmpty();
        address.Number.Should().BeEmpty();
        address.Neighborhood.Should().BeEmpty();
        address.City.Should().BeEmpty();
        address.State.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryAddress_CanSetAllProperties()
    {
        // Act
        var address = new DeliveryAddress
        {
            Cep = "12345-678",
            Street = "Rua das Flores",
            Number = "123",
            Neighborhood = "Centro",
            City = "São Paulo",
            State = "SP"
        };

        // Assert
        address.Cep.Should().Be("12345-678");
        address.Street.Should().Be("Rua das Flores");
        address.Number.Should().Be("123");
        address.Neighborhood.Should().Be("Centro");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
    }

    [Fact]
    public void DeliveryAddress_WithComplexNumber_AcceptsString()
    {
        // Arrange & Act
        var address = new DeliveryAddress
        {
            Number = "123-A"
        };

        // Assert
        address.Number.Should().Be("123-A");
    }

    [Fact]
    public void DeliveryAddress_WithAllFields_RetainsAllData()
    {
        // Arrange
        var cep = "98765-432";
        var street = "Avenida Paulista";
        var number = "1000";
        var neighborhood = "Bela Vista";
        var city = "São Paulo";
        var state = "SP";

        // Act
        var address = new DeliveryAddress
        {
            Cep = cep,
            Street = street,
            Number = number,
            Neighborhood = neighborhood,
            City = city,
            State = state
        };

        // Assert
        address.Cep.Should().Be(cep);
        address.Street.Should().Be(street);
        address.Number.Should().Be(number);
        address.Neighborhood.Should().Be(neighborhood);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
    }
}
