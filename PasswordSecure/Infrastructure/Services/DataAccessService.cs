using System;
using System.Security.Cryptography;
using PasswordSecure.Application.Extensions;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class DataAccessService : IDataAccessService
{
	public DataAccessService(
		IDataSerializationService dataSerializationService,
		IDataEncryptionService dataEncryptionService,
		IFileAccessProvider fileAccessProvider)
	{
		_dataSerializationService = dataSerializationService;
		_dataEncryptionService = dataEncryptionService;
		_fileAccessProvider = fileAccessProvider;
	}
	
	public AccountEntryCollection ReadAccountEntries(AccessParams accessParams)
	{
		try
		{
			var encryptedData = _fileAccessProvider.ReadData(accessParams.FilePath!);

			var data = _dataEncryptionService.DecryptData(encryptedData, accessParams.Password!);
			var serializedData = data.ToText();

			var accountEntries = _dataSerializationService.Deserialize(serializedData);
			return accountEntries;
		}
		catch (CryptographicException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new ApplicationException(FileReadError(accessParams.FilePath!), ex);
		}
	}

	public void SaveAccountEntries(AccessParams accessParams, AccountEntryCollection accountEntryCollection)
	{
		try
		{
			var serializedData = _dataSerializationService.Serialize(accountEntryCollection);
			var data = serializedData.ToByteArray();

			var encryptedData = _dataEncryptionService.EncryptData(
				data, accessParams.Password!);
			
			_fileAccessProvider.SaveData(accessParams.FilePath!, encryptedData);
		}
		catch (CryptographicException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new ApplicationException(FileSaveError(accessParams.FilePath!), ex);
		}
	}
	
	#region Private
	
	private readonly IDataSerializationService _dataSerializationService;
	private readonly IDataEncryptionService _dataEncryptionService;
	private readonly IFileAccessProvider _fileAccessProvider;
	
	private static string FileReadError(string filePath) => $@"Could not read data from file ""{filePath}"".";
	private static string FileSaveError(string filePath) => $@"Could not save data to file ""{filePath}"".";

	#endregion
}
