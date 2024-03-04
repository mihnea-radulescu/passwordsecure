using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataSerializationService
{
	string Serialize(AccountEntryCollection accountEntryCollection);

	AccountEntryCollection Deserialize(string serializedData);
}
