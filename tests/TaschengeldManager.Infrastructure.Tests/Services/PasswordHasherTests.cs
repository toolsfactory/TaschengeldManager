using FluentAssertions;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut;

    public PasswordHasherTests()
    {
        _sut = new PasswordHasher();
    }

    #region Password Tests

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyHash()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash = _sut.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void HashPassword_SamePasswordShouldProduceDifferentHashes()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash1 = _sut.HashPassword(password);
        var hash2 = _sut.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2, "because different salts should produce different hashes");
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePassword123!";
        var invalidHash = "not-a-valid-hash";

        // Act
        var result = _sut.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var result = _sut.VerifyPassword(password, "");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_IsCaseSensitive()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var resultLower = _sut.VerifyPassword("securepassword123!", hash);
        var resultUpper = _sut.VerifyPassword("SECUREPASSWORD123!", hash);

        // Assert
        resultLower.Should().BeFalse();
        resultUpper.Should().BeFalse();
    }

    #endregion

    #region PIN Tests

    [Fact]
    public void HashPin_ShouldReturnNonEmptyHash()
    {
        // Arrange
        var pin = "1234";

        // Act
        var hash = _sut.HashPin(pin);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(pin);
    }

    [Fact]
    public void HashPin_SamePinShouldProduceDifferentHashes()
    {
        // Arrange
        var pin = "1234";

        // Act
        var hash1 = _sut.HashPin(pin);
        var hash2 = _sut.HashPin(pin);

        // Assert
        hash1.Should().NotBe(hash2, "because different salts should produce different hashes");
    }

    [Fact]
    public void VerifyPin_WithCorrectPin_ReturnsTrue()
    {
        // Arrange
        var pin = "1234";
        var hash = _sut.HashPin(pin);

        // Act
        var result = _sut.VerifyPin(pin, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPin_WithIncorrectPin_ReturnsFalse()
    {
        // Arrange
        var pin = "1234";
        var wrongPin = "5678";
        var hash = _sut.HashPin(pin);

        // Act
        var result = _sut.VerifyPin(wrongPin, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPin_WithInvalidHash_ReturnsFalse()
    {
        // Arrange
        var pin = "1234";
        var invalidHash = "not-a-valid-hash";

        // Act
        var result = _sut.VerifyPin(pin, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("0000")]
    [InlineData("9999")]
    public void VerifyPin_WorksWithVariousPinLengths(string pin)
    {
        // Arrange
        var hash = _sut.HashPin(pin);

        // Act
        var result = _sut.VerifyPin(pin, hash);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Cross-Verification Tests

    [Fact]
    public void VerifyPassword_WithPinHash_ReturnsFalse()
    {
        // Arrange
        var value = "1234";
        var pinHash = _sut.HashPin(value);

        // Act - trying to verify as password what was hashed as PIN
        var result = _sut.VerifyPassword(value, pinHash);

        // Assert - should fail because PIN uses different parameters
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPin_WithPasswordHash_ReturnsFalse()
    {
        // Arrange
        var value = "TestPassword123";
        var passwordHash = _sut.HashPassword(value);

        // Act - trying to verify as PIN what was hashed as password
        var result = _sut.VerifyPin(value, passwordHash);

        // Assert - should fail because password uses different parameters
        result.Should().BeFalse();
    }

    #endregion
}
