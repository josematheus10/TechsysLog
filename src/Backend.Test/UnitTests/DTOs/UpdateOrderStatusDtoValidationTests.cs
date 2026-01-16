using Backend.DTOs.Request;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Test.UnitTests.DTOs;

public class UpdateOrderStatusDtoValidationTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void UpdateOrderStatusDto_WithStatusNovo_PassesValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = "novo"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateOrderStatusDto_WithStatusEntregue_PassesValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = "entregue"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateOrderStatusDto_WithEmptyStatus_FailsValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = ""
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("status é obrigatório"));
    }

    [Fact]
    public void UpdateOrderStatusDto_WithInvalidStatus_FailsValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = "em transporte"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("Status inválido"));
    }

    [Fact]
    public void UpdateOrderStatusDto_WithUppercaseStatus_FailsValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = "NOVO"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.ErrorMessage!.Contains("Status inválido"));
    }

    [Fact]
    public void UpdateOrderStatusDto_WithRandomText_FailsValidation()
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = "random-status"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("novo")]
    [InlineData("entregue")]
    public void UpdateOrderStatusDto_WithValidStatuses_PassesValidation(string status)
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = status
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData("pendente")]
    [InlineData("cancelado")]
    [InlineData("em_transporte")]
    [InlineData("processando")]
    public void UpdateOrderStatusDto_WithInvalidStatuses_FailsValidation(string status)
    {
        // Arrange
        var dto = new UpdateOrderStatusDto
        {
            Status = status
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
    }
}
