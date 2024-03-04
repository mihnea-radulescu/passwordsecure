using System.IO;
using PasswordSecure.Application.Helpers;

namespace PasswordSecure.Infrastructure.Helpers;

public class FileAccessProvider : IFileAccessProvider
{
	public byte[] ReadData(string filePath) => File.ReadAllBytes(filePath);

	public void SaveData(string filePath, byte[] data)
		=> File.WriteAllBytes(filePath, data);
}
