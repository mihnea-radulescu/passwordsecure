using System;

namespace PasswordSecure.DomainModel.CustomEventArgs;

public class AccountEntryCollectionEventArgs : EventArgs
{
	public AccountEntryCollectionEventArgs(AccountEntryCollection? accountEntryCollection, bool hasChanged)
	{
		AccountEntryCollection = accountEntryCollection;

		HasChanged = hasChanged;
	}

	public AccountEntryCollection? AccountEntryCollection { get; }

	public bool HasChanged { get; }
}
