namespace PasswordSecure.Logic;

public interface IDataEncryptionService
{
	byte[] EncryptData(string serializedData, string password);
	
	string DecryptData(byte[] encryptedDataBytes, string password);
}
