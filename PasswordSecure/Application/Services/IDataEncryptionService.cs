namespace PasswordSecure.Application.Services;

public interface IDataEncryptionService
{
	byte[] EncryptData(string serializedData, string password);
	
	string DecryptData(byte[] encryptedDataBytes, string password);
}
