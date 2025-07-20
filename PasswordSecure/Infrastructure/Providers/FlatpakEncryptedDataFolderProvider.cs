using System;
using System.IO;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class FlatpakEncryptedDataFolderProvider : IEncryptedDataFolderProvider
{
	public string GetEncryptedDataFolderPath()
	{
		var sandboxedApplicationPath = Environment
			.GetFolderPath(Environment.SpecialFolder.ApplicationData)
			.Replace("/config", string.Empty);

		var encryptedDataFolderPath = Path.Combine(
			sandboxedApplicationPath, EncryptedDataFolderName);
		return encryptedDataFolderPath;
	}

	#region Private

	private const string EncryptedDataFolderName = "EncryptedData";

	#endregion
}
