namespace PasswordSecure.Application.Helpers;

public interface IFileAccessProvider
{
	byte[] ReadData(string filePath);

	void SaveData(string filePath, byte[] data);
}
