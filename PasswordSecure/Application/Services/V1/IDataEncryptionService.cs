namespace PasswordSecure.Application.Services.V1;

public interface IDataEncryptionService
{
    byte[] EncryptData(byte[] data, string password);

    byte[] DecryptData(byte[] encryptedData, string password);
}
