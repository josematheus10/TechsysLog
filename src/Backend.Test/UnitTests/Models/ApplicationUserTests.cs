using Backend.Models;
using FluentAssertions;

namespace Backend.Test.UnitTests.Models;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_DefaultConstructor_CreatesInstance()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.Should().NotBeNull();
        user.FullName.Should().BeNull();
        user.Roles.Should().BeNull();
        user.Logins.Should().BeNull();
        user.Claims.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_CanSetFullName()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.FullName = "John Doe";

        // Assert
        user.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void ApplicationUser_CanSetRoles()
    {
        // Arrange
        var user = new ApplicationUser();
        var roles = new List<string> { "Admin", "User" };

        // Act
        user.Roles = roles;

        // Assert
        user.Roles.Should().NotBeNull();
        user.Roles.Should().HaveCount(2);
        user.Roles.Should().Contain("Admin");
        user.Roles.Should().Contain("User");
    }

    [Fact]
    public void ApplicationUser_InheritsFromIdentityUser()
    {
        // Act
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Assert
        user.Id.Should().NotBeNullOrEmpty();
        user.UserName.Should().Be("testuser");
        user.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void ApplicationUser_WithAllProperties_RetainsAllData()
    {
        // Arrange & Act
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "johndoe",
            Email = "john@example.com",
            FullName = "John Doe",
            Roles = new List<string> { "User", "Manager" }
        };

        // Assert
        user.Id.Should().Be("user-123");
        user.UserName.Should().Be("johndoe");
        user.Email.Should().Be("john@example.com");
        user.FullName.Should().Be("John Doe");
        user.Roles.Should().HaveCount(2);
    }
}
