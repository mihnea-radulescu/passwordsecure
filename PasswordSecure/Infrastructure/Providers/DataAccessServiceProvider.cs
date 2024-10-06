using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Infrastructure.Providers;

public class DataAccessServiceProvider
{
    public static DataAccessService Create()
    {
        return new DataAccessService(
            new FileAccessProvider(),
            new JsonDataSerializationService(),
            new Infrastructure.Services.V1.AesDataEncryptionService(),
            new Infrastructure.Services.V2.AesDataEncryptionService(),
            new BackupService(new FileAccessProvider(), new CurrentDateTimeProvider())
        );
    }
}
