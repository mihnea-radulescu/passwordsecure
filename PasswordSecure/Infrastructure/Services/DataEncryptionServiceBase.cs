using System;
using System.IO;
using System.Security.Cryptography;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public abstract class DataEncryptionServiceBase : IDataEncryptionService
{
	public Vault EncryptDataToVault(byte[] data, string password)
	{
		try
		{
			var iv = GenerateIv();
			var salt = GenerateSalt();

			var key = GetPasswordBytes(password, salt);
			var encryptedData = ExecuteCryptoTransform(data, iv, key, aes => aes.CreateEncryptor());

			var header = new VaultHeader(VaultVersion.V2, iv, salt);
			var vault = new Vault(header, encryptedData);

			return vault;
		}
		catch (Exception ex)
		{
			throw new CryptographicException(EncryptionError, ex);
		}
	}

	public byte[] DecryptDataFromVault(Vault vault, string password)
	{
		try
		{
			var iv = GetIvFromVault(vault);
			var salt = GetSaltFromVault(vault);
			
			var key = GetPasswordBytes(password, salt);

			var encryptedData = vault.Body;
			var data = ExecuteCryptoTransform(encryptedData, iv, key, aes => aes.CreateDecryptor());
			
			return data;
		}
		catch (Exception ex)
		{
			throw new CryptographicException(DecryptionError, ex);
		}
	}
	
	#region Protected

	protected abstract VaultVersion VaultVersion { get; }

	protected abstract int PasswordIterations { get; }

	protected abstract byte[] GenerateIv();
	protected abstract byte[] GenerateSalt();

	protected abstract byte[] GetIvFromVault(Vault vault);
	protected abstract byte[] GetSaltFromVault(Vault vault);

	protected const int KeySizeInBits = 256;
	protected const int KeySizeInBytes = KeySizeInBits / 8;

	protected const string EncryptionError = "Could not encrypt data.";
	protected const string DecryptionError =
		"Could not decrypt data. The likely cause is an incorrect password.";

	protected static byte[] ExecuteCryptoTransform(
		byte[] data,
		byte[] iv,
		byte[] key,
		Func<Aes, ICryptoTransform> createCryptoTransform)
	{
		using var aes = Aes.Create();
		aes.IV = iv;
		aes.KeySize = KeySizeInBits;
		aes.Key = key;

		using var dataStream = new MemoryStream();
		using var encryptor = createCryptoTransform(aes);
		using (var cryptoStream = new CryptoStream(dataStream, encryptor, CryptoStreamMode.Write))
		{
			cryptoStream.Write(data);
		}

		var resultData = dataStream.ToArray();
		return resultData;
	}

	protected byte[] GetPasswordBytes(string password, byte[] salt)
	{
		var derivedBytes = new Rfc2898DeriveBytes(
			password, salt, PasswordIterations, HashAlgorithmName.SHA256);

		var passwordBytes = derivedBytes.GetBytes(KeySizeInBytes);
		return passwordBytes;
	}

	#endregion
}
