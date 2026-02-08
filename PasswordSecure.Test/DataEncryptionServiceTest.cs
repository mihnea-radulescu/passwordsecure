using System.Security.Cryptography;
using Xunit;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Test;

public class DataEncryptionServiceTest
{
	public DataEncryptionServiceTest()
	{
		_dataEncryptionService = new DataEncryptionService();
	}

	[Fact]
	public void EncryptDecrypt_ShortMatchingPassword_ReturnsInitialData()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		var dataReference = serializedDataReference.ToByteArray();

		const string password = "password";

		// Act
		var vault = _dataEncryptionService.EncryptDataToVault(
			dataReference, password);
		var data = _dataEncryptionService.DecryptDataFromVault(vault, password);
		var serializedData = data.ToText();

		// Assert
		Assert.Equal(serializedDataReference, serializedData);
	}

	[Fact]
	public void EncryptDecrypt_LongMatchingPassword_ReturnsInitialData()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		var dataReference = serializedDataReference.ToByteArray();

		const string password = "password_password_password_password";

		// Act
		var vault = _dataEncryptionService.EncryptDataToVault(
			dataReference, password);
		var data = _dataEncryptionService.DecryptDataFromVault(vault, password);
		var serializedData = data.ToText();

		// Assert
		Assert.Equal(serializedDataReference, serializedData);
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
		var vault = _dataEncryptionService.EncryptDataToVault(
			dataReference, encryptionPassword);

		Assert.Throws<CryptographicException>(
			() => _dataEncryptionService.DecryptDataFromVault(
				vault, decryptionPassword)
		);
	}

	[Fact]
	public void EncryptDecrypt_LongNotMatchingPassword_ThrowsCryptographicException()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		var dataReference = serializedDataReference.ToByteArray();

		const string encryptionPassword =
			"encryption password_password_password_password";
		const string decryptionPassword =
			"decryption password_password_password_password";

		// Act and Assert
		var vault = _dataEncryptionService.EncryptDataToVault(
			dataReference, encryptionPassword);

		Assert.Throws<CryptographicException>(
			() => _dataEncryptionService.DecryptDataFromVault(
				vault, decryptionPassword)
		);
	}

	private readonly DataEncryptionService _dataEncryptionService;
}
