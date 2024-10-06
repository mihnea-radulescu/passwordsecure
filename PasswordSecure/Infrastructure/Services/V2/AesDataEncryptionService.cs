using System;
using System.IO;
using System.Security.Cryptography;
using PasswordSecure.Application.Services.V2;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services.V2;

public class AesDataEncryptionService : IDataEncryptionService
{
    public Vault EncryptNewVault(byte[] data, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return EncryptVault(data, password, salt);
    }

    public Vault EncryptVault(byte[] data, string password, byte[] salt)
    {
        try
        {
            var key = GetPasswordBytes(password, salt);
            var iv = RandomNumberGenerator.GetBytes(16);
            var encryptedData = ExecuteCryptoTransform(data, iv, key, aes => aes.CreateEncryptor());

            return new Vault()
            {
                Header = new VaultHeader()
                {
                    Version = VaultVersion.V2,
                    IV = iv,
                    Salt = salt,
                },
                Body = encryptedData,
            };
        }
        catch (Exception ex)
        {
            throw new CryptographicException(EncryptionError, ex);
        }
    }

    public byte[] DecryptVault(Vault vault, string password)
    {
        try
        {
            var key = GetPasswordBytes(password, vault.Header.Salt);
            var iv = vault.Header.IV;
            var data = ExecuteCryptoTransform(vault.Body, iv, key, aes => aes.CreateDecryptor());

            return data;
        }
        catch (Exception ex)
        {
            throw new CryptographicException(DecryptionError, ex);
        }
    }

    #region Private

    // See: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2
    private const int PasswordIterations = 600_000;
    private const int KeySizeInBits = 256;
    private const int KeySizeInBytes = KeySizeInBits / 8;
    private const string EncryptionError = "Could not encrypt data.";
    private const string DecryptionError =
        "Could not decrypt data. The likely cause is an incorrect password.";

    private static byte[] ExecuteCryptoTransform(
        byte[] data,
        byte[] iv,
        byte[] key,
        Func<Aes, ICryptoTransform> createCryptoTransform
    )
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
        var derivedBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            PasswordIterations,
            HashAlgorithmName.SHA256
        );

        var passwordBytes = derivedBytes.GetBytes(KeySizeInBytes);
        return passwordBytes;
    }

    #endregion
}
