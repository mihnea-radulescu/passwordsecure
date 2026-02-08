using System.IO;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class FileAccessProvider : IFileAccessProvider
{
	public byte[] ReadData(string filePath) => File.ReadAllBytes(filePath);

	public void SaveData(string filePath, byte[] data)
		=> File.WriteAllBytes(filePath, data);

	public void CopyFile(string sourceFilePath, string destinationFilePath)
		=> File.Copy(sourceFilePath, destinationFilePath, true);
}
