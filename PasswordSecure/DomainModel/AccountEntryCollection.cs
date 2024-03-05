using System.Collections.Generic;

namespace PasswordSecure.DomainModel;

public class AccountEntryCollection : List<AccountEntry>
{
	public AccountEntryCollection()
		: base()
	{
	}

	public AccountEntryCollection(ICollection<AccountEntry> accountEntries)
		: base(accountEntries)
	{
	}
}
