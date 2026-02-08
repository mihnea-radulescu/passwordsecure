using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataSerializationService
{
	string SerializeVault(Vault vault);
	Vault DeserializeVault(string serializedVault);

	string SerializeAccountEntryCollection(
		AccountEntryCollection accountEntryCollection);
	AccountEntryCollection DeserializeAccountEntryCollection(
		string serializedAccountEntryCollection);
}
