using Backend.Configuration;
using FluentAssertions;

namespace Backend.Test.UnitTests.Configuration;

public class JwtSettingsTests
{
    [Fact]
    public void JwtSettings_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var settings = new JwtSettings();

        // Assert
        settings.SecretKey.Should().BeEmpty();
        settings.Issuer.Should().BeEmpty();
        settings.Audience.Should().BeEmpty();
        settings.ExpirationInMinutes.Should().Be(60);
    }

    [Fact]
    public void JwtSettings_CanSetAllProperties()
    {
        // Act
        var settings = new JwtSettings
        {
            SecretKey = "test-secret-key",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInMinutes = 120
        };

        // Assert
        settings.SecretKey.Should().Be("test-secret-key");
        settings.Issuer.Should().Be("TestIssuer");
        settings.Audience.Should().Be("TestAudience");
        settings.ExpirationInMinutes.Should().Be(120);
    }

    [Fact]
    public void JwtSettings_ExpirationInMinutes_AcceptsDifferentValues()
    {
        // Arrange
        var settings = new JwtSettings();

        // Act & Assert
        settings.ExpirationInMinutes = 30;
        settings.ExpirationInMinutes.Should().Be(30);

        settings.ExpirationInMinutes = 1440; // 24 hours
        settings.ExpirationInMinutes.Should().Be(1440);
    }

    [Fact]
    public void JwtSettings_WithAllRequiredProperties_IsValid()
    {
        // Arrange & Act
        var settings = new JwtSettings
        {
            SecretKey = "my-super-secret-key-with-at-least-32-characters",
            Issuer = "MyApp",
            Audience = "MyAppUsers",
            ExpirationInMinutes = 60
        };

        // Assert
        settings.SecretKey.Should().NotBeNullOrEmpty();
        settings.Issuer.Should().NotBeNullOrEmpty();
        settings.Audience.Should().NotBeNullOrEmpty();
        settings.ExpirationInMinutes.Should().BeGreaterThan(0);
    }
}
