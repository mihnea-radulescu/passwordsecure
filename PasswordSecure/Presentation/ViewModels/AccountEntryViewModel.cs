using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryViewModel : ObservableObject, IPasswordViewModel
{
	public AccountEntryViewModel(AccountEntry accountEntry)
	{
		AccountEntry = accountEntry;
	}

	public AccountEntry AccountEntry { get; }

	public string Name
	{
		get => AccountEntry.Name;
		set
		{
			SetProperty(
				AccountEntry.Name,
				value,
				AccountEntry,
				(accountEntry, propertyValue)
					=> accountEntry.Name = propertyValue);
		}
	}

	public string Url
	{
		get => AccountEntry.Url;
		set
		{
			SetProperty(
				AccountEntry.Url,
				value,
				AccountEntry,
				(accountEntry, propertyValue)
					=> accountEntry.Url = propertyValue);
		}
	}

	public string User
	{
		get => AccountEntry.User;
		set
		{
			SetProperty(
				AccountEntry.User,
				value,
				AccountEntry,
				(accountEntry, propertyValue)
					=> accountEntry.User = propertyValue);
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
				(accountEntry, propertyValue)
					=> accountEntry.Password = propertyValue);
		}
	}

	public string Notes
	{
		get => AccountEntry.Notes;
		set
		{
			SetProperty(
				AccountEntry.Notes,
				value,
				AccountEntry,
				(accountEntry, propertyValue)
					=> accountEntry.Notes = propertyValue);
		}
	}
}
