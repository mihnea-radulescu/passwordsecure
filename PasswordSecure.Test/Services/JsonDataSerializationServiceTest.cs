using System;
using FluentAssertions;
using NSubstitute;
using Xunit;
using PasswordSecure.Application.Helpers;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Test.TestAttributes;

namespace PasswordSecure.Test.Services;

[UnitTestClass]
public class JsonDataSerializationServiceTest
{
	public JsonDataSerializationServiceTest()
	{
		_jsonDataSerializationService = new JsonDataSerializationService();

		_dateTimeProvider = Substitute.For<IDateTimeProvider>();
		_dateTimeProvider.Now.Returns(DateTime.Parse("2024-01-01T15:30:00"));
	}
	
	#region Serialize
	
	[Fact]
	public void Serialize_NoElementCollection_ReturnsExpectedResult()
	{
		// Arrange
		var accountEntryCollectionReference = new AccountEntryCollection();
		
		// Act
		var serializedData = _jsonDataSerializationService.Serialize(accountEntryCollectionReference);

		// Assert
		const string expectedData = @"{""NameToAccountEntryMapping"":{}}";
		serializedData.Should().Be(expectedData);
	}
	
	[Fact]
	public void Serialize_OneElementCollectionDefaultValues_ReturnsExpectedResult()
	{
		// Arrange
		var accountEntry = new AccountEntry("Google");
		var accountEntryCollectionReference = new AccountEntryCollection();
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry, _dateTimeProvider.Now);
		
		// Act
		var serializedData = _jsonDataSerializationService.Serialize(accountEntryCollectionReference);

		// Assert
		const string expectedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":null,""User"":null,""Password"":null,""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";
		serializedData.Should().Be(expectedData);
	}
	
	[Fact]
	public void Serialize_OneElementCollectionCustomValues_ReturnsExpectedResult()
	{
		// Arrange
		var accountEntry = new AccountEntry("Google")
		{
			Website = "https://mail.google.com",
			User = "john.doe",
			Password = "123456**&&"
		};
		
		var accountEntryCollectionReference = new AccountEntryCollection();
		accountEntryCollectionReference.AddOrUpdateAccountEntry(accountEntry, _dateTimeProvider.Now);
		
		// Act
		var serializedData = _jsonDataSerializationService.Serialize(accountEntryCollectionReference);

		// Assert
		const string expectedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":""https://mail.google.com"",""User"":""john.doe"",""Password"":""123456**\u0026\u0026"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";
		serializedData.Should().Be(expectedData);
	}
	
	[Fact]
	public void Serialize_TwoElementCollectionCustomValues_ReturnsExpectedResult()
	{
		// Arrange
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
		var serializedData = _jsonDataSerializationService.Serialize(accountEntryCollectionReference);

		// Assert
		const string expectedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":""https://mail.google.com"",""User"":""john.doe"",""Password"":""123456**\u0026\u0026"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null},""Microsoft"":{""Name"":""Microsoft"",""Website"":""https://azure.microsoft.com"",""User"":""john_doe"",""Password"":""654321\u0026\u0026**"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";
		serializedData.Should().Be(expectedData);
	}
	
	#endregion
	
	#region Deserialize
	
	[Fact]
	public void Deserialize_NoElementCollection_ReturnsExpectedResult()
	{
		// Arrange
		const string serializedData = @"{""NameToAccountEntryMapping"":{}}";
		
		// Act
		var accountEntryCollection = _jsonDataSerializationService.Deserialize(serializedData);

		// Assert
		accountEntryCollection.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Should().NotBeNull();
		accountEntryCollection.NameToAccountEntryMapping.Count.Should().Be(0);
	}
	
	[Fact]
	public void Deserialize_OneElementCollectionDefaultValues_ReturnsExpectedResult()
	{
		// Arrange
		const string serializedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":null,""User"":null,""Password"":null,""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";
		
		// Act
		var accountEntryCollection = _jsonDataSerializationService.Deserialize(serializedData);

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
	public void Deserialize_OneElementCollectionCustomValues_ReturnsExpectedResult()
	{
		// Arrange
		const string serializedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":""https://mail.google.com"",""User"":""john.doe"",""Password"":""123456**\u0026\u0026"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";

		// Act
		var accountEntryCollection = _jsonDataSerializationService.Deserialize(serializedData);

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
	public void Deserialize_TwoElementCollectionCustomValues_ReturnsExpectedResult()
	{
		// Arrange
		const string serializedData =
			@"{""NameToAccountEntryMapping"":{""Google"":{""Name"":""Google"",""Website"":""https://mail.google.com"",""User"":""john.doe"",""Password"":""123456**\u0026\u0026"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null},""Microsoft"":{""Name"":""Microsoft"",""Website"":""https://azure.microsoft.com"",""User"":""john_doe"",""Password"":""654321\u0026\u0026**"",""DateAdded"":""2024-01-01T15:30:00"",""DateChanged"":null}}}";

		// Act
		var accountEntryCollection = _jsonDataSerializationService.Deserialize(serializedData);

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
	
	#endregion
	
	#region Private

	private readonly JsonDataSerializationService _jsonDataSerializationService;
	
	private readonly IDateTimeProvider _dateTimeProvider;

	#endregion
}
