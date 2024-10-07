using System.Text.Json;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Infrastructure.Services;

public class JsonDataSerializationService : IDataSerializationService
{
	static JsonDataSerializationService()
	{
		JsonSerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true
		};
	}
	
	public string SerializeVault(Vault vault) => Serialize(vault);

    public Vault DeserializeVault(string serializedVault) => Deserialize<Vault>(serializedVault);
	
	public string SerializeAccountEntryCollection(AccountEntryCollection accountEntryCollection)
		=> Serialize(accountEntryCollection);

	public AccountEntryCollection DeserializeAccountEntryCollection(
		string serializedAccountEntryCollection)
		=> Deserialize<AccountEntryCollection>(serializedAccountEntryCollection);

	#region Private

	private static readonly JsonSerializerOptions JsonSerializerOptions;

	private static string Serialize<T>(T instance)
		=> JsonSerializer.Serialize(instance, JsonSerializerOptions);

	private static T Deserialize<T>(string serializedInstance)
		=> JsonSerializer.Deserialize<T>(serializedInstance, JsonSerializerOptions)!;

	#endregion
}
