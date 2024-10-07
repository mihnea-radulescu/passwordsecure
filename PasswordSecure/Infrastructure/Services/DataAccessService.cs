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
		IDataEncryptionService v1dataEncryptionService,
		IDataEncryptionService v2dataEncryptionService,
		IBackupService backupService)
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
		var encryptedDataAsBinary = _fileAccessProvider.ReadData(accessParams.FilePath!);

		var header = new VaultHeader(VaultVersion.V1, [], []);
		var vault = new Vault(header, encryptedDataAsBinary);

		var data = _v1dataEncryptionService.DecryptDataFromVault(vault, accessParams.Password!);
		var serializedData = data.ToText();

		var accountEntries = _dataSerializationService.DeserializeAccountEntryCollection(serializedData);
		return await Task.FromResult(accountEntries);
	}

	public async Task<AccountEntryCollection> ReadV2AccountEntries(AccessParams accessParams)
	{
		var vaultAsBinary = _fileAccessProvider.ReadData(accessParams.FilePath!);
		var vaultAsText = Encoding.UTF8.GetString(vaultAsBinary);

		var vault = _dataSerializationService.DeserializeVault(vaultAsText);
		accessParams.Salt = vault.Header.Salt;

		var data = _v2dataEncryptionService.DecryptDataFromVault(vault, accessParams.Password!);
		var serializedData = data.ToText();

		var accountEntries = _dataSerializationService.DeserializeAccountEntryCollection(serializedData);
		return await Task.FromResult(accountEntries);
	}

	public async Task SaveAccountEntries(
		AccessParams accessParams, AccountEntryCollection accountEntryCollection)
	{
		try
		{
			try
			{
				_backupService.BackupFile(accessParams.FilePath!);
			}
			catch (BackupException)
			{
			}

			var serializedData = _dataSerializationService.SerializeAccountEntryCollection(
				accountEntryCollection);
			var data = serializedData.ToByteArray();

			var vault = _v2dataEncryptionService.EncryptDataToVault(data, accessParams.Password!);

			var vaultAsText = _dataSerializationService.SerializeVault(vault);
			var vaultAsBinary = Encoding.UTF8.GetBytes(vaultAsText);

			_fileAccessProvider.SaveData(accessParams.FilePath!, vaultAsBinary);
			await Task.CompletedTask;
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
	private readonly IDataEncryptionService _v1dataEncryptionService;
	private readonly IDataEncryptionService _v2dataEncryptionService;
	private readonly IBackupService _backupService;

	private static string FileReadError(string filePath) =>
		$@"Could not read data from file ""{filePath}"".";

	private static string FileSaveError(string filePath) =>
		$@"Could not save data to file ""{filePath}"".";

	#endregion
}
