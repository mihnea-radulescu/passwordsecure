using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryViewModel : ObservableObject
{
	public AccountEntryViewModel(AccountEntry accountEntry)
	{
		AccountEntry = accountEntry;
	}
	
	public AccountEntry AccountEntry { get; }

	public event EventHandler? NameChanged;

	public string Name
	{
		get => AccountEntry.Name;
		set
		{
			SetProperty(
				AccountEntry.Name,
				value,
				AccountEntry,
				(accountEntry, propertyValue) => accountEntry.Name = propertyValue);
			
			NameChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public string? Website
	{
		get => AccountEntry.Website;
		set
		{
			SetProperty(
				AccountEntry.Website,
				value,
				AccountEntry,
				(accountEntry, propertyValue) => accountEntry.Website = propertyValue);
		}
	}
	
	public string? User
	{
		get => AccountEntry.User;
		set
		{
			SetProperty(
				AccountEntry.User,
				value,
				AccountEntry,
				(accountEntry, propertyValue) => accountEntry.User = propertyValue);
		}
	}
	
	public string? Password
	{
		get => AccountEntry.Password;
		set
		{
			SetProperty(
				AccountEntry.Password,
				value,
				AccountEntry,
				(accountEntry, propertyValue) => accountEntry.Password = propertyValue);
		}
	}
}
