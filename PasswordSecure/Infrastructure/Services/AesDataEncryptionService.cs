using System;
using System.IO;
using System.Security.Cryptography;
using PasswordSecure.Application.Services;

namespace PasswordSecure.Infrastructure.Services;

public class AesDataEncryptionService : IDataEncryptionService
{
	public byte[] EncryptData(byte[] data, string password)
	{
		try
		{
			var encryptedData = ExecuteCryptoTransform(
				data, password, aes => aes.CreateEncryptor());

			return encryptedData;
		}
		catch (Exception ex)
		{
			throw new CryptographicException(EncryptionError, ex);
		}
	}

	public byte[] DecryptData(byte[] encryptedData, string password)
	{
		try
		{
			var data = ExecuteCryptoTransform(
				encryptedData, password, aes => aes.CreateDecryptor());
		
			return data;
		}
		catch (Exception ex)
		{
			throw new CryptographicException(DecryptionError, ex);
		}
	}
	
	#region Private

	private const int PasswordIterations = 16;
	private const int KeySizeInBits = 256;
	private const int PasswordSizeInBytes = KeySizeInBits / 8;
	private const string EncryptionError = "Could not encrypt data.";
	private const string DecryptionError = "Could not decrypt data. The probable cause is an incorrect password.";
	
	private static readonly byte[] Salt =
	{
		0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
		0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
	};
	
	private static readonly byte[] InitializationVector =
	{
		0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10, 0x09,
		0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01
	};
	
	private static byte[] ExecuteCryptoTransform(
		byte[] data, string password, Func<Aes, ICryptoTransform> createCryptoTransform)
	{
		var passwordBytes = GetPasswordBytes(password);
		
		using var aes = Aes.Create();
		aes.IV = InitializationVector;
		aes.KeySize = KeySizeInBits;
		aes.Key = passwordBytes;
		
		using var dataStream = new MemoryStream();
		using var encryptor = createCryptoTransform(aes);
		using (var cryptoStream = new CryptoStream(dataStream, encryptor, CryptoStreamMode.Write))
		{
			cryptoStream.Write(data);
		}
		
		var resultData = dataStream.ToArray();
		return resultData;
	}

	private static byte[] GetPasswordBytes(string password)
	{
		var derivedBytes = new Rfc2898DeriveBytes(
			password, Salt, PasswordIterations, HashAlgorithmName.SHA256);
		
		var passwordBytes = derivedBytes.GetBytes(PasswordSizeInBytes);
		return passwordBytes;
	}

	#endregion
}
