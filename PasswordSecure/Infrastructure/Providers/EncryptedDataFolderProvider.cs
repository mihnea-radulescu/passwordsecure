using System;
using System.IO;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class EncryptedDataFolderProvider : IEncryptedDataFolderProvider
{
	public EncryptedDataFolderProvider(
		IEnvironmentSettingsProvider environmentSettingsProvider)
	{
		_environmentSettingsProvider = environmentSettingsProvider;
	}

	private const string FlatpakEncryptedDataFolderName = "EncryptedData";

	private readonly IEnvironmentSettingsProvider _environmentSettingsProvider;

	public string GetEncryptedDataFolderPath()
	{
		string encryptedDataFolderPath;

		if (_environmentSettingsProvider.IsInsideFlatpakContainer)
		{
			var userFlatpakConfigFolderPath = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData);

			try
			{
				var userFlatpakConfigFolderInfo = new DirectoryInfo(
					userFlatpakConfigFolderPath);

				var userFlatpakAppFolderInfo =
					userFlatpakConfigFolderInfo.Parent;
				var userFlatpakAppFolderPath =
					userFlatpakAppFolderInfo!.FullName;

				encryptedDataFolderPath = Path.Combine(
					userFlatpakAppFolderPath, FlatpakEncryptedDataFolderName);
			}
			catch
			{
				encryptedDataFolderPath = userFlatpakConfigFolderPath;
			}
		}
		else
		{
			encryptedDataFolderPath =
				Environment.GetFolderPath(
					Environment.SpecialFolder.UserProfile);
		}

		return encryptedDataFolderPath;
	}
}
