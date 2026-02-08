namespace PasswordSecure.DomainModel;

public record BackupInfo(
	string BackupFolderPath,
	string BackupFilePath,
	string BackupFolderPrefix,
	string FileExtension);
