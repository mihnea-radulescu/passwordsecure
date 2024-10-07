using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataEncryptionService
{
	Vault EncryptDataToVault(byte[] data, string password);

	byte[] DecryptDataFromVault(Vault encryptedData, string password);
}
