using System.IO;
using System.Threading.Tasks;
using Xunit;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Providers;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Test;

public class TaskDecoratorDataAccessServiceTest
{
	public TaskDecoratorDataAccessServiceTest()
	{
		IFileAccessProvider fileAccessProvider = new FileAccessProvider();
		IDateTimeProvider dateTimeProvider = new CurrentDateTimeProvider();

		IDataSerializationService dataSerializationService =
			new JsonDataSerializationService();
		var dataEncryptionService = new DataEncryptionService();
		IBackupService backupService = new BackupService(
			fileAccessProvider, dateTimeProvider);

		IDataAccessService dataAccessService = new DataAccessService(
			fileAccessProvider,
			dataSerializationService,
			dataEncryptionService,
			backupService);

		_taskDecoratorDataAccessService = new TaskDecoratorDataAccessService(
			dataAccessService);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_NoElementCollection_ReturnsInitialData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_NoElementCollection.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		var accountEntryCollectionReference = new AccountEntryCollection();

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollection);
		Assert.Empty(accountEntryCollection);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_OneElementCollectionDefaultValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_OneElementCollectionDefaultValues.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		const string name = "Google";

		var accountEntry = new AccountEntry { Name = name };

		var accountEntryCollectionReference =
			new AccountEntryCollection { accountEntry };

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollection);
		Assert.Single(accountEntryCollection);

		var singleAccountEntry = accountEntryCollection[0];

		Assert.NotNull(singleAccountEntry);
		Assert.Equal(name, singleAccountEntry.Name);
		Assert.Empty(singleAccountEntry.Url);
		Assert.Empty(singleAccountEntry.User);
		Assert.Null(singleAccountEntry.Password);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_OneElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_OneElementCollectionCustomValues.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		const string name = "Google";
		const string url = "https://mail.google.com";
		const string user = "john.doe";
		const string password = "123456**&&";

		var accountEntry = new AccountEntry
		{
			Name = name,
			Url = url,
			User = user,
			Password = password,
		};

		var accountEntryCollectionReference =
			new AccountEntryCollection { accountEntry };

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollection);
		Assert.Single(accountEntryCollection);

		var singleAccountEntry = accountEntryCollection[0];

		Assert.NotNull(singleAccountEntry);
		Assert.Equal(name, singleAccountEntry.Name);
		Assert.Equal(url, singleAccountEntry.Url);
		Assert.Equal(user, singleAccountEntry.User);
		Assert.Equal(password, singleAccountEntry.Password);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		const string name1 = "Google";
		const string url1 = "https://mail.google.com";
		const string user1 = "john.doe";
		const string password1 = "123456**&&";

		var accountEntry1 = new AccountEntry
		{
			Name = name1,
			Url = url1,
			User = user1,
			Password = password1,
		};

		const string name2 = "Microsoft";
		const string url2 = "https://azure.microsoft.com";
		const string user2 = "john_doe";
		const string password2 = "654321&&**";

		var accountEntry2 = new AccountEntry
		{
			Name = name2,
			Url = url2,
			User = user2,
			Password = password2,
		};

		var accountEntryCollectionReference = new AccountEntryCollection
		{
			accountEntry1,
			accountEntry2,
		};

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollection);
		Assert.Equal(2, accountEntryCollection.Count);

		var firstAccountEntry = accountEntryCollection[0];

		Assert.NotNull(firstAccountEntry);
		Assert.Equal(name1, firstAccountEntry.Name);
		Assert.Equal(url1, firstAccountEntry.Url);
		Assert.Equal(user1, firstAccountEntry.User);
		Assert.Equal(password1, firstAccountEntry.Password);

		var secondAccountEntry = accountEntryCollection[1];

		Assert.NotNull(secondAccountEntry);
		Assert.Equal(name2, secondAccountEntry.Name);
		Assert.Equal(url2, secondAccountEntry.Url);
		Assert.Equal(user2, secondAccountEntry.User);
		Assert.Equal(password2, secondAccountEntry.Password);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValuesOneEntryModified_ReturnsChangedData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		const string name1 = "Google";
		const string url1 = "https://mail.google.com";
		const string user1 = "john.doe";
		const string password1 = "123456**&&";

		var accountEntry1 = new AccountEntry
		{
			Name = name1,
			Url = url1,
			User = user1,
			Password = password1,
		};

		const string name2 = "Microsoft";
		const string url2 = "https://azure.microsoft.com";
		const string user2 = "john_doe";
		const string password2 = "654321&&**";

		var accountEntry2 = new AccountEntry
		{
			Name = name2,
			Url = url2,
			User = user2,
			Password = password2,
		};

		const string name1Changed = "Google Mail";
		const string url1Changed = "https://mail.google.com/";
		const string user1Changed = "jane.doe";
		const string password1Changed = "123456**&&~~";

		var accountEntryCollectionReference = new AccountEntryCollection
		{
			accountEntry1,
			accountEntry2,
		};

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		accountEntryCollection[0].Name = name1Changed;
		accountEntryCollection[0].Url = url1Changed;
		accountEntryCollection[0].User = user1Changed;
		accountEntryCollection[0].Password = password1Changed;

		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollection);
		var accountEntryCollectionChanged =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollectionChanged);
		Assert.Equal(2, accountEntryCollectionChanged.Count);

		var firstAccountEntryChanged = accountEntryCollectionChanged[0];

		Assert.NotNull(firstAccountEntryChanged);
		Assert.Equal(name1Changed, firstAccountEntryChanged.Name);
		Assert.Equal(url1Changed, firstAccountEntryChanged.Url);
		Assert.Equal(user1Changed, firstAccountEntryChanged.User);
		Assert.Equal(password1Changed, firstAccountEntryChanged.Password);

		var secondAccountEntryChanged = accountEntryCollectionChanged[1];

		Assert.NotNull(secondAccountEntryChanged);
		Assert.Equal(name2, secondAccountEntryChanged.Name);
		Assert.Equal(url2, secondAccountEntryChanged.Url);
		Assert.Equal(user2, secondAccountEntryChanged.User);
		Assert.Equal(password2, secondAccountEntryChanged.Password);
	}

	[Fact]
	public async Task SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValuesOneEntryReplaced_ReturnsChangedData()
	{
		// Arrange
		const string fileName =
			"MyPasswordContainer_TwoElementCollectionDefaultValues.encrypted";
		var filePath = GetFilePath(fileName);

		var accessParams = new AccessParams
		{ 
			FilePath = filePath,
			Password = Password
		};

		const string name1 = "Google";
		const string url1 = "https://mail.google.com";
		const string user1 = "john.doe";
		const string password1 = "123456**&&";

		var accountEntry1 = new AccountEntry
		{
			Name = name1,
			Url = url1,
			User = user1,
			Password = password1,
		};

		const string name2 = "Microsoft";
		const string url2 = "https://azure.microsoft.com";
		const string user2 = "john_doe";
		const string password2 = "654321&&**";

		var accountEntry2 = new AccountEntry
		{
			Name = name2,
			Url = url2,
			User = user2,
			Password = password2,
		};

		const string name1Changed = "Google Mail";
		const string url1Changed = "https://mail.google.com/";
		const string user1Changed = "jane.doe";
		const string password1Changed = "123456**&&~~";

		var accountEntry1Changed = new AccountEntry
		{
			Name = name1Changed,
			Url = url1Changed,
			User = user1Changed,
			Password = password1Changed,
		};

		var accountEntryCollectionReference = new AccountEntryCollection
		{
			accountEntry1,
			accountEntry2,
		};

		// Act
		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollectionReference);
		var accountEntryCollection =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		accountEntryCollection[0] = accountEntry1Changed;

		await _taskDecoratorDataAccessService.SaveAccountEntries(
			accessParams, accountEntryCollection);
		var accountEntryCollectionChanged =
			await _taskDecoratorDataAccessService.ReadAccountEntries(
				accessParams);

		// Assert
		Assert.NotNull(accountEntryCollectionChanged);
		Assert.Equal(2, accountEntryCollectionChanged.Count);

		var firstAccountEntryChanged = accountEntryCollectionChanged[0];

		Assert.NotNull(firstAccountEntryChanged);
		Assert.Equal(name1Changed, firstAccountEntryChanged.Name);
		Assert.Equal(url1Changed, firstAccountEntryChanged.Url);
		Assert.Equal(user1Changed, firstAccountEntryChanged.User);
		Assert.Equal(password1Changed, firstAccountEntryChanged.Password);

		var secondAccountEntryChanged = accountEntryCollectionChanged[1];

		Assert.NotNull(secondAccountEntryChanged);
		Assert.Equal(name2, secondAccountEntryChanged.Name);
		Assert.Equal(url2, secondAccountEntryChanged.Url);
		Assert.Equal(user2, secondAccountEntryChanged.User);
		Assert.Equal(password2, secondAccountEntryChanged.Password);
	}

	private const string Password = "Master Password";

	private readonly TaskDecoratorDataAccessService
		_taskDecoratorDataAccessService;

	private static string GetFilePath(string fileName)
		=> Path.GetFullPath(fileName);
}
