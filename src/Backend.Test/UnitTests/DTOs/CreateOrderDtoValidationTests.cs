using Backend.DTOs.Request;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Test.UnitTests.DTOs;

public class CreateOrderDtoValidationTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CreateOrderDto_WithValidData_PassesValidation()
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
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOrderDto_WithMissingOrderNumber_FailsValidation()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "",
            Description = "Test Order",
            Value = 100.50m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("número do pedido"));
    }

    [Fact]
    public void CreateOrderDto_WithZeroValue_FailsValidation()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Value = 0m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("maior que zero"));
    }

    [Fact]
    public void CreateOrderDto_WithNegativeValue_FailsValidation()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Value = -10m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateOrderDto_WithTooLongOrderNumber_FailsValidation()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = new string('A', 51),
            Description = "Test Order",
            Value = 100m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("50 caracteres"));
    }

    [Fact]
    public void CreateOrderDto_WithTooLongDescription_FailsValidation()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = new string('A', 501),
            Value = 100m,
            DeliveryAddress = new DeliveryAddressDto
            {
                Cep = "12345-678",
                Street = "Test Street",
                Number = "123",
                Neighborhood = "Test",
                City = "TestCity",
                State = "SP"
            }
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("500 caracteres"));
    }
}
