using PasswordSecure.Model;

namespace PasswordSecure.Logic;

public interface IDataSerializer
{
	string Serialize(AccountEntryCollection accountEntryCollection);

	AccountEntryCollection Deserialize(string serializedData);
}
