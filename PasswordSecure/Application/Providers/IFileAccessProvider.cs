namespace PasswordSecure.Application.Providers;

public interface IFileAccessProvider
{
	byte[] ReadData(string filePath);

	void SaveData(string filePath, byte[] data);

	void CopyFile(string sourceFilePath, string destinationFilePath);

	void DeleteFile(string filePath);
}
