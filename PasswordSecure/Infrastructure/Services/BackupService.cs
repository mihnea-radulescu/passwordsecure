using System.IO;
using System.Linq;
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

	public void BackupFile(string filePath, bool isV1Vault)
	{
		try
		{
			if (!ExistsFile(filePath))
			{
				return;
			}
			
			var backupInfo = GetBackupInfo(filePath);

			CreateFolderIfNecessary(backupInfo.BackupFolderPath);

			if (isV1Vault)
			{
				DeleteExistingBackupFiles(backupInfo);
			}
			else
			{
				_fileAccessProvider.CopyFile(filePath, backupInfo.BackupFilePath);
			}
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

	private void DeleteExistingBackupFiles(BackupInfo backupInfo)
	{
		var backupFolder = new DirectoryInfo(backupInfo.BackupFolderPath);
		
		var backupFilesSearchPattern = $"*{backupInfo.FileExtension}";
		var backupFiles = backupFolder.GetFiles(backupFilesSearchPattern);

		var backupFilesToDelete = backupFiles
			.Where(aBackupFile => aBackupFile.Name.StartsWith(backupInfo.BackupFolderPrefix))
			.Select(aBackupFile => aBackupFile.FullName)
			.ToList();

		foreach (var aBackupFileToDelete in backupFilesToDelete)
		{
			try
			{
				_fileAccessProvider.DeleteFile(aBackupFileToDelete);
			}
			catch
			{
			}
		}
	}

    #endregion
}
