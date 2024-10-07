using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Infrastructure.Providers;

public class DataAccessServiceProvider : IDataAccessServiceProvider
{
	public IDataAccessService CreateDataAccessService()
	{
		IDataSerializationService jsonDataSerializationService = new JsonDataSerializationService();

		IDataEncryptionService aesV1DataEncryptionService = new AesV1DataEncryptionService();
		IDataEncryptionService aesV2DataEncryptionService = new AesV2DataEncryptionService();

		IFileAccessProvider fileAccessProvider = new FileAccessProvider();
		IDateTimeProvider currentDateTimeProvider = new CurrentDateTimeProvider();
		IBackupService backupService = new BackupService(fileAccessProvider, currentDateTimeProvider);
		
		IDataAccessService dataAccessService = new DataAccessService(
			fileAccessProvider,
			jsonDataSerializationService,
			aesV1DataEncryptionService,
			aesV2DataEncryptionService,
			backupService);

		return dataAccessService;
	}
}
