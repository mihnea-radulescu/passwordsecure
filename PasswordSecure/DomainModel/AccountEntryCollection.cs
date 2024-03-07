using System.Collections.Generic;

namespace PasswordSecure.DomainModel;

public class AccountEntryCollection : List<AccountEntry>
{
	public AccountEntryCollection()
	{
	}

	public AccountEntryCollection(ICollection<AccountEntry> accountEntries)
		: base(accountEntries)
	{
	}
}
