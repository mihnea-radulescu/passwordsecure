using System.Text.Json;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class JsonDataSerializationService : IDataSerializationService
{
	public string Serialize(AccountEntryCollection accountEntryCollection)
	{
		var serializedData = JsonSerializer.Serialize(accountEntryCollection);
		return serializedData;
	}

	public AccountEntryCollection Deserialize(string serializedData)
	{
		var accountEntryCollection = JsonSerializer.Deserialize<AccountEntryCollection>(serializedData)!;
		return accountEntryCollection;
	}
}
