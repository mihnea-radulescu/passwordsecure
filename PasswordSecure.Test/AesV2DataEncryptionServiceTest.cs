using System.Security.Cryptography;
using FluentAssertions;
using Xunit;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Test;

public class AesV2DataEncryptionServiceTest
{
	public AesV2DataEncryptionServiceTest()
	{
		_aesV2DataEncryptionService = new AesV2DataEncryptionService();
	}

	[Fact]
	public void EncryptDecrypt_ShortMatchingPassword_ReturnsInitialData()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		var dataReference = serializedDataReference.ToByteArray();

		const string password = "password";

		// Act
		var vault = _aesV2DataEncryptionService.EncryptDataToVault(dataReference, password);
		var data = _aesV2DataEncryptionService.DecryptDataFromVault(vault, password);
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
		var vault = _aesV2DataEncryptionService.EncryptDataToVault(dataReference, password);
		var data = _aesV2DataEncryptionService.DecryptDataFromVault(vault, password);
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
		var vault = _aesV2DataEncryptionService.EncryptDataToVault(dataReference, encryptionPassword);

		Assert.Throws<CryptographicException>(
			() => _aesV2DataEncryptionService.DecryptDataFromVault(vault, decryptionPassword)
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
		var vault = _aesV2DataEncryptionService.EncryptDataToVault(dataReference, encryptionPassword);

		Assert.Throws<CryptographicException>(
			() => _aesV2DataEncryptionService.DecryptDataFromVault(vault, decryptionPassword)
		);
	}

	#region Private

	private readonly AesV2DataEncryptionService _aesV2DataEncryptionService;

	#endregion
}
