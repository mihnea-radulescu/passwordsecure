using System.IO;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public partial class BackupService : IBackupService
{
	public BackupService(
		IFileAccessProvider fileAccessProvider,
		IDateTimeProvider dateTimeProvider)
	{
		_fileAccessProvider = fileAccessProvider;
		_dateTimeProvider = dateTimeProvider;
	}

	public void BackupFile(string filePath)
	{
		try
		{
			if (!ExistsFile(filePath))
			{
				return;
			}

			var backupInfo = GetBackupInfo(filePath);

			CreateFolderIfNecessary(backupInfo.BackupFolderPath);
			_fileAccessProvider.CopyFile(filePath, backupInfo.BackupFilePath);
		}
		catch
		{
		}
	}

	#region Private

	private const string BackupFolderSuffix = "Backup";

	private readonly IFileAccessProvider _fileAccessProvider;
	private readonly IDateTimeProvider _dateTimeProvider;

	private static bool ExistsFile(string filePath) => Path.Exists(filePath);

	private static void CreateFolderIfNecessary(string backupFolderPath)
	{
		if (!Directory.Exists(backupFolderPath))
		{
			Directory.CreateDirectory(backupFolderPath);
		}
	}

	private BackupInfo GetBackupInfo(string filePath)
	{
		var now = _dateTimeProvider.Now;

		var fileName = Path.GetFileName(filePath);
		var fileExtension = Path.GetExtension(fileName);

		var backupFolderRootPath = Path.GetDirectoryName(filePath)!;
		var backupFolderPrefix = Path.GetFileNameWithoutExtension(fileName);
		var backupFolderName = $"{backupFolderPrefix}_{BackupFolderSuffix}";
		var backupFolderPath = Path.Combine(backupFolderRootPath, backupFolderName);

		var backupFileName = $"{backupFolderPrefix}_{now}{fileExtension}";
		var backupFilePath = Path.Combine(backupFolderPath, backupFileName);

		var backupInfo = new BackupInfo(
			backupFolderPath, backupFilePath, backupFolderPrefix, fileExtension);

		return backupInfo;
	}

	#endregion
}
