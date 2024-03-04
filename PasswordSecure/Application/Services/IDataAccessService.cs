using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataAccessService
{
	AccountEntryCollection ReadAccountEntries(string filePath, string masterPassword);
	
	void SaveAccountEntries(string filePath, string masterPassword, AccountEntryCollection accountEntries);
}
