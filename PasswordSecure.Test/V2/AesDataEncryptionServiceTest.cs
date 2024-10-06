using System.Security.Cryptography;
using FluentAssertions;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Infrastructure.Services.V2;
using Xunit;

namespace PasswordSecure.Test.V2;

public class AesDataEncryptionServiceTest
{
    public AesDataEncryptionServiceTest()
    {
        _aesDataEncryptionService = new AesDataEncryptionService();
    }

    private static readonly byte[] Salt = RandomNumberGenerator.GetBytes(16);

    [Fact]
    public void EncryptDecrypt_ShortMatchingPassword_ReturnsInitialData()
    {
        // Arrange
        const string serializedDataReference = "plain text";
        var dataReference = serializedDataReference.ToByteArray();

        const string password = "password";

        // Act
        var encryptedDataBytes = _aesDataEncryptionService.EncryptVault(
            dataReference,
            password,
            Salt
        );
        var data = _aesDataEncryptionService.DecryptVault(encryptedDataBytes, password);
        var serializedData = data.ToText();

        // Assert
        serializedData.Should().Be(serializedDataReference);
    }

    [Fact]
    public void EncryptDecrypt_LongMatchingPassword_ReturnsInitialData()
    {
        // Arrange
        const string serializedDataReference = "plain text";
        var dataReference = serializedDataReference.ToByteArray();

        const string password = "password_password_password_password";

        // Act
        var encryptedDataBytes = _aesDataEncryptionService.EncryptVault(
            dataReference,
            password,
            Salt
        );
        var data = _aesDataEncryptionService.DecryptVault(encryptedDataBytes, password);
        var serializedData = data.ToText();

        // Assert
        serializedData.Should().Be(serializedDataReference);
    }

    [Fact]
    public void EncryptDecrypt_ShortNotMatchingPassword_ThrowsCryptographicException()
    {
        // Arrange
        const string serializedDataReference = "plain text";
        var dataReference = serializedDataReference.ToByteArray();

        const string encryptionPassword = "encryption password";
        const string decryptionPassword = "decryption password";

        // Act and Assert
        var encryptedData = _aesDataEncryptionService.EncryptVault(
            dataReference,
            encryptionPassword,
            Salt
        );

        Assert.Throws<CryptographicException>(
            () => _aesDataEncryptionService.DecryptVault(encryptedData, decryptionPassword)
        );
    }

    [Fact]
    public void EncryptDecrypt_LongNotMatchingPassword_ThrowsCryptographicException()
    {
        // Arrange
        const string serializedDataReference = "plain text";
        var dataReference = serializedDataReference.ToByteArray();

        const string encryptionPassword = "encryption password_password_password_password";
        const string decryptionPassword = "decryption password_password_password_password";

        // Act and Assert
        var encryptedData = _aesDataEncryptionService.EncryptVault(
            dataReference,
            encryptionPassword,
            Salt
        );

        Assert.Throws<CryptographicException>(
            () => _aesDataEncryptionService.DecryptVault(encryptedData, decryptionPassword)
        );
    }

    #region Private

    private readonly AesDataEncryptionService _aesDataEncryptionService;

    #endregion
}
