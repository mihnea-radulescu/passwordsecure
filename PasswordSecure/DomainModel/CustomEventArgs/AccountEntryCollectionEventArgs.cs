using System;

namespace PasswordSecure.DomainModel.CustomEventArgs;

public class AccountEntryCollectionEventArgs : EventArgs
{
	public AccountEntryCollectionEventArgs(
		AccountEntryCollection accountEntryCollection)
	{
		AccountEntryCollection = accountEntryCollection;
	}
	
	public AccountEntryCollection AccountEntryCollection { get; }
}
