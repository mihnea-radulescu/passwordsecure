using System;
using FluentAssertions;
using NSubstitute;
using Xunit;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Helpers;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Test.TestAttributes;

namespace PasswordSecure.Test.Services;

[IntegrationTestClass]
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
		
		_dateTimeProvider = Substitute.For<IDateTimeProvider>();
		_dateTimeProvider.Now.Returns(DateTime.Parse("2024-01-01T15:30:00"));
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_NoElementCollection_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_NoElementCollection.data";
		
		var accountEntryCollectionReference = new AccountEntryCollection();
		
		// Act
		_dataAccessService.SaveAccountEntries(fileName, MasterPassword, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(fileName, MasterPassword);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Count.Should().Be(0);
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_OneElementCollectionDefaultValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_OneElementCollectionDefaultValues.data";
		
		var accountEntry = new AccountEntry("Google");
		var accountEntryCollectionReference = new AccountEntryCollection();
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry, _dateTimeProvider.Now);
		
		// Act
		_dataAccessService.SaveAccountEntries(fileName, MasterPassword, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(fileName, MasterPassword);

		// Assert
		const string nameKey = "Google";
		var dateAdded = DateTime.Parse("2024-01-01T15:30:00");
		
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Count.Should().Be(1);

		accountEntryCollection.NameToAccountEntryMapping.Should().ContainKey(nameKey);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Name.Should().Be(nameKey);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Website.Should().BeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey].User.Should().BeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Password.Should().BeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey].DateAdded.Should().Be(dateAdded);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].DateChanged.Should().BeNull();
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_OneElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_OneElementCollectionCustomValues.data";
		
		var accountEntry = new AccountEntry("Google")
		{
			Website = "https://mail.google.com",
			User = "john.doe",
			Password = "123456**&&"
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection();
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry, _dateTimeProvider.Now);
		
		// Act
		_dataAccessService.SaveAccountEntries(fileName, MasterPassword, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(fileName, MasterPassword);

		// Assert
		const string nameKey = "Google";
		const string website = "https://mail.google.com";
		const string user = "john.doe";
		const string password = "123456**&&";
		var dateAdded = DateTime.Parse("2024-01-01T15:30:00");
		
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Count.Should().Be(1);

		accountEntryCollection.NameToAccountEntryMapping.Should().ContainKey(nameKey);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Name.Should().Be(nameKey);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Website.Should().Be(website);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].User.Should().Be(user);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].Password.Should().Be(password);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].DateAdded.Should().Be(dateAdded);
		accountEntryCollection.NameToAccountEntryMapping[nameKey].DateChanged.Should().BeNull();
	}
	
	[Fact]
	public void SaveAccountEntriesReadAccountEntries_TwoElementCollectionCustomValues_ReturnsInitialData()
	{
		// Arrange
		const string fileName = "Encrypted_TwoElementCollectionCustomValues.data";
		
		var accountEntry1 = new AccountEntry("Google")
		{
			Website = "https://mail.google.com",
			User = "john.doe",
			Password = "123456**&&"
		};
		
		var accountEntry2 = new AccountEntry("Microsoft")
		{
			Website = "https://azure.microsoft.com",
			User = "john_doe",
			Password = "654321&&**"
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection();
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry1, _dateTimeProvider.Now);
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry2, _dateTimeProvider.Now);
		
		// Act
		_dataAccessService.SaveAccountEntries(fileName, MasterPassword, accountEntryCollectionReference);
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(fileName, MasterPassword);

		// Assert
		var dateAdded = DateTime.Parse("2024-01-01T15:30:00");
		
		const string nameKey1 = "Google";
		const string website1 = "https://mail.google.com";
		const string user1 = "john.doe";
		const string password1 = "123456**&&";
		
		const string nameKey2 = "Microsoft";
		const string website2 = "https://azure.microsoft.com";
		const string user2 = "john_doe";
		const string password2 = "654321&&**";
		
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Count.Should().Be(2);
		
		accountEntryCollection.NameToAccountEntryMapping.Should().ContainKey(nameKey1);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].Name.Should().Be(nameKey1);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].Website.Should().Be(website1);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].User.Should().Be(user1);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].Password.Should().Be(password1);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].DateAdded.Should().Be(dateAdded);
		accountEntryCollection.NameToAccountEntryMapping[nameKey1].DateChanged.Should().BeNull();

		accountEntryCollection.NameToAccountEntryMapping.Should().ContainKey(nameKey2);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].Name.Should().Be(nameKey2);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].Website.Should().Be(website2);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].User.Should().Be(user2);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].Password.Should().Be(password2);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].DateAdded.Should().Be(dateAdded);
		accountEntryCollection.NameToAccountEntryMapping[nameKey2].DateChanged.Should().BeNull();
	}

	#region Private

	private const string MasterPassword = "Master Password";

	private readonly DataAccessService _dataAccessService;
	
	private readonly IDateTimeProvider _dateTimeProvider;

	#endregion
}
