using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services.V2;

public interface IDataEncryptionService
{
    Vault EncryptNewVault(byte[] data, string password);

    Vault EncryptVault(byte[] data, string password, byte[] salt);

    byte[] DecryptVault(Vault encryptedData, string password);
}
