using FluentAssertions;
using Xunit;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Helpers;
using PasswordSecure.Infrastructure.Services;

namespace PasswordSecure.Test;

public class DataAccessServiceTest
{
	public DataAccessServiceTest()
	{
		IDataSerializationService dataSerializationService = new JsonDataSerializationService();
		IDataEncryptionService dataEncryptionService = new AesDataEncryptionService();
		IFileAccessProvider fileAccessProvider = new FileAccessProvider();

		_dataAccessService = new DataAccessService(
			dataSerializationService,
			dataEncryptionService,
			fileAccessProvider);
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_NoElementCollection_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_NoElementCollection.data";
		var accessParams = new AccessParams
		{
			FilePath = fileName,
			Password = Password
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection();
		
		// Act
		_dataAccessService.SaveAccountEntries(accessParams, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(accessParams);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.Should().BeEmpty();
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_OneElementCollectionDefaultValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_OneElementCollectionDefaultValues.data";
		var accessParams = new AccessParams
		{
			FilePath = fileName,
			Password = Password
		};

		const string name = "Google";
		
		var accountEntry = new AccountEntry
		{
			Name = name
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection { accountEntry };
		
		// Act
		_dataAccessService.SaveAccountEntries(accessParams, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(accessParams);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.Count.Should().Be(1);
		
		var singleAccountEntry = accountEntryCollection[0];
		
		singleAccountEntry.Should().NotBeNull();
		singleAccountEntry.Name.Should().Be(name);
		singleAccountEntry.Url.Should().BeNull();
		singleAccountEntry.User.Should().BeNull();
		singleAccountEntry.Password.Should().BeNull();
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_OneElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_OneElementCollectionCustomValues.data";
		var accessParams = new AccessParams
		{
			FilePath = fileName,
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
			Password = password
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection { accountEntry };
		
		// Act
		_dataAccessService.SaveAccountEntries(accessParams, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(accessParams);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.Count.Should().Be(1);
		
		var singleAccountEntry = accountEntryCollection[0];
		
		singleAccountEntry.Should().NotBeNull();
		singleAccountEntry.Name.Should().Be(name);
		singleAccountEntry.Url.Should().Be(url);
		singleAccountEntry.User.Should().Be(user);
		singleAccountEntry.Password.Should().Be(password);
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_TwoElementCollectionCustomValues.data";
		var accessParams = new AccessParams
		{
			FilePath = fileName,
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
			Password = password1
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
			Password = password2
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection { accountEntry1, accountEntry2 };
		
		// Act
		_dataAccessService.SaveAccountEntries(accessParams, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(accessParams);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.Count.Should().Be(2);
		
		var firstAccountEntry = accountEntryCollection[0];
		
		firstAccountEntry.Should().NotBeNull();
		firstAccountEntry.Name.Should().Be(name1);
		firstAccountEntry.Url.Should().Be(url1);
		firstAccountEntry.User.Should().Be(user1);
		firstAccountEntry.Password.Should().Be(password1);
		
		var secondAccountEntry = accountEntryCollection[1];
		
		secondAccountEntry.Should().NotBeNull();
		secondAccountEntry.Name.Should().Be(name2);
		secondAccountEntry.Url.Should().Be(url2);
		secondAccountEntry.User.Should().Be(user2);
		secondAccountEntry.Password.Should().Be(password2);
	}

	#region Private

	private const string Password = "Master Password";

	private readonly DataAccessService _dataAccessService;

	#endregion
}