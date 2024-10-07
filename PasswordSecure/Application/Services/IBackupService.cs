namespace PasswordSecure.Application.Services;

public interface IBackupService
{
	void BackupFile(string filePath, bool isV1Vault);
}
