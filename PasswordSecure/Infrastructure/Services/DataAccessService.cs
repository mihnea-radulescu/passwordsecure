using System;
using System.Security.Cryptography;
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
	
	public AccountEntryCollection ReadAccountEntries(string filePath, string masterPassword)
	{
		try
		{
			var encryptedData = _fileAccessProvider.ReadData(filePath);

			var serializedData = _dataEncryptionService.DecryptData(encryptedData, masterPassword);

			var accountEntries = _dataSerializationService.Deserialize(serializedData);
			return accountEntries;
		}
		catch (CryptographicException cEx)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new ApplicationException(FileReadError(filePath), ex);
		}
	}

	public void SaveAccountEntries(string filePath, string masterPassword, AccountEntryCollection accountEntries)
	{
		try
		{
			var serializedData = _dataSerializationService.Serialize(accountEntries);

			var encryptedData = _dataEncryptionService.EncryptData(serializedData, masterPassword);
			
			_fileAccessProvider.SaveData(filePath, encryptedData);
		}
		catch (CryptographicException cEx)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new ApplicationException(FileSaveError(filePath), ex);
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
