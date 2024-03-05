using PasswordSecure.DomainModel;

namespace PasswordSecure.Application.Services;

public interface IDataAccessService
{
	AccountEntryCollection ReadAccountEntries(AccessParams accessParams);
	
	void SaveAccountEntries(AccessParams accessParams, AccountEntryCollection accountEntryCollection);
}
