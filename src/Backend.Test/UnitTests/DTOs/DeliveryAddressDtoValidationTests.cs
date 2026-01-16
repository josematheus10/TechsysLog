using Backend.DTOs.Request;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Test.UnitTests.DTOs;

public class DeliveryAddressDtoValidationTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void DeliveryAddressDto_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryAddressDto_WithValidCepWithoutHyphen_PassesValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryAddressDto_WithInvalidCep_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "123",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("CEP inválido"));
    }

    [Fact]
    public void DeliveryAddressDto_WithMissingStreet_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("rua"));
    }

    [Fact]
    public void DeliveryAddressDto_WithMissingNumber_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("número"));
    }

    [Fact]
    public void DeliveryAddressDto_WithTooLongStreet_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = new string('A', 201),
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("200 caracteres"));
    }

    [Fact]
    public void DeliveryAddressDto_WithInvalidState_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "TestCity",
            State = "S" // Only 1 character
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("2 caracteres"));
    }

    [Fact]
    public void DeliveryAddressDto_WithMissingCity_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "Test",
            City = "",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("cidade"));
    }

    [Fact]
    public void DeliveryAddressDto_WithMissingNeighborhood_FailsValidation()
    {
        // Arrange
        var dto = new DeliveryAddressDto
        {
            Cep = "12345-678",
            Street = "Test Street",
            Number = "123",
            Neighborhood = "",
            City = "TestCity",
            State = "SP"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("bairro"));
    }
}
