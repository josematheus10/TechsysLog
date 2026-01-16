using Backend.DTOs.Request;
using Backend.Mappers;
using Backend.Models;
using FluentAssertions;

namespace Backend.Test.UnitTests.Mappers;

public class DeliveryAddressMapperTests
{
    [Fact]
    public void ToModel_WithValidDto_ReturnsDeliveryAddressModel()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test Neighborhood",
            City = "Test City",
            State = "sp"
        };

        // Act
        var model = dto.ToModel();

        // Assert
        model.Should().NotBeNull();
        model.Cep.Should().Be(dto.Cep);
        model.Street.Should().Be(dto.Street);
        model.Number.Should().Be(dto.Number);
        model.Neighborhood.Should().Be(dto.Neighborhood);
        model.City.Should().Be(dto.City);
        model.State.Should().Be("SP"); // Should be uppercase
    }

    [Fact]
    public void ToModel_WithLowercaseState_ConvertsToUppercase()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "98765-432",
            Street = "Another Street",
            Number = "456",
            Neighborhood = "Another Neighborhood",
            City = "Another City",
            State = "rj"
        };

        // Act
        var model = dto.ToModel();

        // Assert
        model.State.Should().Be("RJ");
    }

    [Fact]
    public void ToModel_WithUppercaseState_KeepsUppercase()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "11111-222",
            Street = "Test Street",
            Number = "789",
            Neighborhood = "Test Neighborhood",
            City = "Test City",
            State = "MG"
        };

        // Act
        var model = dto.ToModel();

        // Assert
        model.State.Should().Be("MG");
    }

    [Fact]
    public void ToDto_WithValidModel_ReturnsDeliveryAddressDto()
    {
        // Arrange
        var model = new DeliveryAddress
        {
            Cep = "55555-666",
            Street = "Model Street",
            Number = "999",
            Neighborhood = "Model Neighborhood",
            City = "Model City",
            State = "BA"
        };

        // Act
        var dto = model.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Cep.Should().Be(model.Cep);
        dto.Street.Should().Be(model.Street);
        dto.Number.Should().Be(model.Number);
        dto.Neighborhood.Should().Be(model.Neighborhood);
        dto.City.Should().Be(model.City);
        dto.State.Should().Be(model.State);
    }

    [Fact]
    public void ToDto_PreservesAllFields()
    {
        // Arrange
        var model = new DeliveryAddress
        {
            Cep = "88888-999",
            Street = "Complete Street Name",
            Number = "12A",
            Neighborhood = "Downtown",
            City = "Big City",
            State = "RS"
        };

        // Act
        var dto = model.ToDto();

        // Assert
        dto.Cep.Should().Be("88888-999");
        dto.Street.Should().Be("Complete Street Name");
        dto.Number.Should().Be("12A");
        dto.Neighborhood.Should().Be("Downtown");
        dto.City.Should().Be("Big City");
        dto.State.Should().Be("RS");
    }

    [Fact]
    public void RoundTripConversion_PreservesData()
    {
        // Arrange
        var originalDto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "100",
            Neighborhood = "Test",
            City = "TestCity",
            State = "sp"
        };

        // Act
        var model = originalDto.ToModel();
        var resultDto = model.ToDto();

        // Assert
        resultDto.Cep.Should().Be(originalDto.Cep);
        resultDto.Street.Should().Be(originalDto.Street);
        resultDto.Number.Should().Be(originalDto.Number);
        resultDto.Neighborhood.Should().Be(originalDto.Neighborhood);
        resultDto.City.Should().Be(originalDto.City);
        resultDto.State.Should().Be("SP"); // Uppercase after round trip
    }
}
