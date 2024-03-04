using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordSecure.Logic.Implementation;

public class AesDataEncryptionService : IDataEncryptionService
{
	public byte[] EncryptData(string serializedData, string password)
	{
		try
		{
			var serializedDataBytes = Encoding.GetBytes(serializedData);

			var encryptedDataBytes = ExecuteCryptoTransform(
				serializedDataBytes, password, aes => aes.CreateEncryptor());

			return encryptedDataBytes;
		}
		catch (Exception ex)
		{
			throw new CryptographicException(EncryptionError, ex);
		}
	}

	public string DecryptData(byte[] encryptedDataBytes, string password)
	{
		try
		{
			var serializedDataBytes = ExecuteCryptoTransform(
				encryptedDataBytes, password, aes => aes.CreateDecryptor());

			var serializedData = Encoding.GetString(serializedDataBytes);
		
			return serializedData;
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
	
	private static readonly Encoding Encoding = Encoding.UTF8;
	
	private static byte[] ExecuteCryptoTransform(
		byte[] inputData, string password, Func<Aes, ICryptoTransform> createCryptoTransform)
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
			cryptoStream.Write(inputData);
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
