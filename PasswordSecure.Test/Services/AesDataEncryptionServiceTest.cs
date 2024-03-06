using System.Security.Cryptography;
using FluentAssertions;
using PasswordSecure.Application.Extensions;
using Xunit;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Test.Services;

public class AesDataEncryptionServiceTest
{
	public AesDataEncryptionServiceTest()
	{
		_aesDataEncryptionService = new AesDataEncryptionService();
	}

	[Fact]
	public void EncryptDecrypt_ShortMatchingPassword_ReturnsInitialData()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		var dataReference = serializedDataReference.ToByteArray();
		
		const string password = "password";

		// Act
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(dataReference, password);
		var data = _aesDataEncryptionService.DecryptData(encryptedDataBytes, password);
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
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(dataReference, password);
		var data = _aesDataEncryptionService.DecryptData(encryptedDataBytes, password);
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
		var encryptedData = _aesDataEncryptionService.EncryptData(dataReference, encryptionPassword);
		
		Assert.Throws<CryptographicException>(() =>
			_aesDataEncryptionService.DecryptData(encryptedData, decryptionPassword));
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
		var encryptedData = _aesDataEncryptionService.EncryptData(dataReference, encryptionPassword);
		
		Assert.Throws<CryptographicException>(() =>
			_aesDataEncryptionService.DecryptData(encryptedData, decryptionPassword));
	}
	
	#region Private

	private readonly AesDataEncryptionService _aesDataEncryptionService;

	#endregion
}
