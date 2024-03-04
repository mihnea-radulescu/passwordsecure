using System.Security.Cryptography;
using FluentAssertions;
using Xunit;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Test.TestAttributes;

namespace PasswordSecure.Test.Services;

[UnitTestClass]
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
		const string password = "password";

		// Act
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(serializedDataReference, password);
		var serializedData = _aesDataEncryptionService.DecryptData(encryptedDataBytes, password);

		// Assert
		serializedData.Should().Be(serializedDataReference);
	}
	
	[Fact]
	public void EncryptDecrypt_LongMatchingPassword_ReturnsInitialData()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		const string password = "password_password_password_password";

		// Act
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(serializedDataReference, password);
		var serializedData = _aesDataEncryptionService.DecryptData(encryptedDataBytes, password);

		// Assert
		serializedData.Should().Be(serializedDataReference);
	}
	
	[Fact]
	public void EncryptDecrypt_ShortNotMatchingPassword_ThrowsCryptographicException()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		const string encryptionPassword = "encryption password";
		const string decryptionPassword = "decryption password";

		// Act and Assert
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(serializedDataReference, encryptionPassword);
		
		Assert.Throws<CryptographicException>(() =>
			_aesDataEncryptionService.DecryptData(encryptedDataBytes, decryptionPassword));
	}
	
	[Fact]
	public void EncryptDecrypt_LongNotMatchingPassword_ThrowsCryptographicException()
	{
		// Arrange
		const string serializedDataReference = "plain text";
		const string encryptionPassword = "encryption password_password_password_password";
		const string decryptionPassword = "decryption password_password_password_password";

		// Act and Assert
		var encryptedDataBytes = _aesDataEncryptionService.EncryptData(serializedDataReference, encryptionPassword);
		
		Assert.Throws<CryptographicException>(() =>
			_aesDataEncryptionService.DecryptData(encryptedDataBytes, decryptionPassword));
	}
	
	#region Private

	private readonly AesDataEncryptionService _aesDataEncryptionService;

	#endregion
}
