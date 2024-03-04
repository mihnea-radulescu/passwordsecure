using System.Text.Json;
using PasswordSecure.Model;

namespace PasswordSecure.Logic.Implementation;

public class JsonDataSerializer : IDataSerializer
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
