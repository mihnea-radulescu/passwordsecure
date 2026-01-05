using System;
using System.IO;
using System.Security.Cryptography;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class DataEncryptionService : IDataEncryptionService
{
	public Vault EncryptDataToVault(byte[] data, string password)
	{
		try
		{
			var iv = GenerateIv();
			var salt = GenerateSalt();

			var key = GetPasswordBytes(password, salt);
			var encryptedData = ExecuteCryptoTransform(data, iv, key, aes => aes.CreateEncryptor());

			var header = new VaultHeader(iv, salt);
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

	private const int KeySizeInBits = 256;
	private const int KeySizeInBytes = KeySizeInBits / 8;

	private const int IvSizeInBytes = 16;
	private const int SaltSizeInBytes = 16;

	// See: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2
	private const int PasswordIterations = 600_000;

	private const string EncryptionError = "Could not encrypt data.";
	private const string DecryptionError = "Could not decrypt data. The likely cause is an incorrect password.";

	private static byte[] GenerateIv() => RandomNumberGenerator.GetBytes(IvSizeInBytes);
	private static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSizeInBytes);

	private static byte[] GetIvFromVault(Vault vault) => vault.Header.IV;
	private static byte[] GetSaltFromVault(Vault vault) => vault.Header.Salt;

	private static byte[] ExecuteCryptoTransform(
		byte[] data, byte[] iv, byte[] key, Func<Aes, ICryptoTransform> createCryptoTransform)
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

	private static byte[] GetPasswordBytes(string password, byte[] salt)
	{
		var passwordBytes = Rfc2898DeriveBytes.Pbkdf2(
			password, salt, PasswordIterations, HashAlgorithmName.SHA256, KeySizeInBytes);

		return passwordBytes;
	}
}
