using System;
using System.Collections.Generic;

namespace PasswordSecure.Model;

public class AccountEntryCollection
{
	public AccountEntryCollection()
	{
		NameToAccountEntryMapping = new SortedDictionary<string, AccountEntry>();
	}
	
	public IDictionary<string, AccountEntry> NameToAccountEntryMapping { get; set; }

	public void AddOrUpdateAccountEntry(AccountEntry accountEntry, DateTime now)
	{
		if (NameToAccountEntryMapping.ContainsKey(accountEntry.Name))
		{
			accountEntry.DateChanged = now;
		}
		else
		{
			accountEntry.DateAdded = now;
		}
		
		NameToAccountEntryMapping[accountEntry.Name] = accountEntry;
	}

	public void DeleteAccountEntry(AccountEntry accountEntry)
		=> NameToAccountEntryMapping.Remove(accountEntry.Name);

	public AccountEntry UpdateName(AccountEntry accountEntry, string newAccountEntryName, DateTime now)
	{
		DeleteAccountEntry(accountEntry);

		var newAccountEntry = CreateAccountEntry(accountEntry, newAccountEntryName);
		AddOrUpdateAccountEntry(newAccountEntry, now);

		return newAccountEntry;
	}
	
	#region Private

	private static AccountEntry CreateAccountEntry(AccountEntry accountEntry, string newAccountEntryName)
	{
		var newAccountEntry = new AccountEntry(newAccountEntryName)
		{
			Website = accountEntry.Website,
			User = accountEntry.User,
			Password = accountEntry.Password
		};

		return newAccountEntry;
	}
	
	#endregion
}
