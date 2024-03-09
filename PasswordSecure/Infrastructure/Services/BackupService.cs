using System;
using System.IO;
using PasswordSecure.Application.Exceptions;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;

namespace PasswordSecure.Infrastructure.Services;

public class BackupService : IBackupService
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
			
			var (backupFolderPath, backupFilePath) = GetBackupInfo(filePath);

			CreateFolder(backupFolderPath);
		
			_fileAccessProvider.CopyFile(filePath, backupFilePath);
		}
		catch (Exception ex)
		{
			throw new BackupException(BackupError(filePath), ex);
		}
	}

	#region Private

	private const string BackupFolderSuffix = "Backup";
	
	private static string BackupError(string filePath) => $@"Could not backup file ""{filePath}"".";

	private readonly IFileAccessProvider _fileAccessProvider;
	private readonly IDateTimeProvider _dateTimeProvider;

	private static bool ExistsFile(string filePath)
		=> Path.Exists(filePath);
	
	private static void CreateFolder(string backupFolderPath)
	{
		if (!Directory.Exists(backupFolderPath))
		{
			Directory.CreateDirectory(backupFolderPath);
		}
	}

	private (string, string) GetBackupInfo(string filePath)
	{
		var now = _dateTimeProvider.Now;
		
		var fileName = Path.GetFileName(filePath);
		var fileExtension = Path.GetExtension(fileName);
		var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		
		var backupFolderRootPath = Path.GetDirectoryName(filePath)!;
		var backupFolderName = $"{fileNameWithoutExtension}_{BackupFolderSuffix}";
		var backupFolderPath = Path.Combine(backupFolderRootPath, backupFolderName);

		var backupFileName = $"{fileNameWithoutExtension}_{now}{fileExtension}";
		var backupFilePath = Path.Combine(backupFolderPath, backupFileName);
		
		return (backupFolderPath, backupFilePath);
	}
	
	#endregion
}
