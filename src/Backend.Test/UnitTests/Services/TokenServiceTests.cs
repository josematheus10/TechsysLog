using Backend.Configuration;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Test.UnitTests.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "this-is-a-very-secure-secret-key-with-at-least-32-characters",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInMinutes = 60
        };

        var options = Options.Create(_jwtSettings);
        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser",
            FullName = "Test User"
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        jwtToken.Should().NotBeNull();
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateToken_WithMultipleRoles_IncludesAllRoles()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "admin@example.com",
            UserName = "admin"
        };
        var roles = new List<string> { "User", "Admin", "Manager" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        roleClaims.Should().HaveCount(3);
        roleClaims.Should().Contain("User");
        roleClaims.Should().Contain("Admin");
        roleClaims.Should().Contain("Manager");
    }

    [Fact]
    public void GenerateToken_WithoutFullName_DoesNotIncludeNameClaim()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser",
            FullName = null
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().NotContain(c => c.Type == ClaimTypes.Name);
    }

    [Fact]
    public void GenerateToken_WithFullName_IncludesNameClaim()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser",
            FullName = "Test User"
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "Test User");
    }

    [Fact]
    public void GenerateToken_WithNoRoles_CreatesTokenWithoutRoleClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser"
        };
        var roles = new List<string>();

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateToken_ChecksExpiration()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser"
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }
}
