namespace PasswordSecure.Application.Services;

public interface IDataEncryptionService
{
	byte[] EncryptData(byte[] data, string password);
	
	byte[] DecryptData(byte[] encryptedData, string password);
}
