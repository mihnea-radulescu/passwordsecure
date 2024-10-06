using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PasswordSecure.Application.Exceptions;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class DataAccessService : IDataAccessService
{
    public DataAccessService(
        IFileAccessProvider fileAccessProvider,
        IDataSerializationService dataSerializationService,
        PasswordSecure.Application.Services.V1.IDataEncryptionService v1dataEncryptionService,
        PasswordSecure.Application.Services.V2.IDataEncryptionService v2dataEncryptionService,
        IBackupService backupService
    )
    {
        _fileAccessProvider = fileAccessProvider;

        _dataSerializationService = dataSerializationService;
        _v1dataEncryptionService = v1dataEncryptionService;
        _v2dataEncryptionService = v2dataEncryptionService;
        _backupService = backupService;
    }

    public async Task<AccountEntryCollection> ReadAccountEntries(AccessParams accessParams)
    {
        try
        {
            try
            {
                return await ReadV2AccountEntries(accessParams);
            }
            catch (JsonException)
            {
                // Probably v1 format (or corrupted)
                return await ReadV1AccountEntries(accessParams);
            }
        }
        catch (CryptographicException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(FileReadError(accessParams.FilePath!), ex);
        }
    }

    public async Task<AccountEntryCollection> ReadV1AccountEntries(AccessParams accessParams)
    {
        var encryptedData = _fileAccessProvider.ReadData(accessParams.FilePath!);

        var data = _v1dataEncryptionService.DecryptData(encryptedData, accessParams.Password!);
        var serializedData = data.ToText();

        var accountEntries = _dataSerializationService.Deserialize(serializedData);
        return await Task.FromResult(accountEntries);
    }

    public async Task<AccountEntryCollection> ReadV2AccountEntries(AccessParams accessParams)
    {
        var encryptedData = _fileAccessProvider.ReadData(accessParams.FilePath!);
        var vault = JsonSerializer.Deserialize<Vault>(Encoding.UTF8.GetString(encryptedData))!;
        accessParams.Salt = vault.Header.Salt;

        var data = _v2dataEncryptionService.DecryptVault(vault, accessParams.Password!);
        var serializedData = data.ToText();

        var accountEntries = _dataSerializationService.Deserialize(serializedData);
        return await Task.FromResult(accountEntries);
    }

    public async Task SaveAccountEntries(
        AccessParams accessParams,
        AccountEntryCollection accountEntryCollection
    )
    {
        try
        {
            try
            {
                _backupService.BackupFile(accessParams.FilePath!);
            }
            catch (BackupException) { }

            var serializedData = _dataSerializationService.Serialize(accountEntryCollection);
            var data = serializedData.ToByteArray();

            Vault? vault;
            if (accessParams.Salt != null)
            {
                vault = _v2dataEncryptionService.EncryptVault(
                    data,
                    accessParams.Password!,
                    accessParams.Salt
                );
            }
            else
            {
                vault = _v2dataEncryptionService.EncryptNewVault(data, accessParams.Password!);
            }

            var encryptedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vault));
            _fileAccessProvider.SaveData(accessParams.FilePath!, encryptedData);
            await Task.CompletedTask;
        }
        catch (BackupException)
        {
            throw;
        }
        catch (CryptographicException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataAccessException(FileSaveError(accessParams.FilePath!), ex);
        }
    }

    #region Private

    private readonly IFileAccessProvider _fileAccessProvider;

    private readonly IDataSerializationService _dataSerializationService;
    private readonly PasswordSecure.Application.Services.V1.IDataEncryptionService _v1dataEncryptionService;
    private readonly PasswordSecure.Application.Services.V2.IDataEncryptionService _v2dataEncryptionService;
    private readonly IBackupService _backupService;

    private static string FileReadError(string filePath) =>
        $@"Could not read data from file ""{filePath}"".";

    private static string FileSaveError(string filePath) =>
        $@"Could not save data to file ""{filePath}"".";

    #endregion
}
