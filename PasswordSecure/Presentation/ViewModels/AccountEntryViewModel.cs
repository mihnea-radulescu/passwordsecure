using System;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryViewModel : ViewModelBase
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
			AccountEntry.Name = value;
			
			OnPropertyChanged();
			NameChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public string? Website
	{
		get => AccountEntry.Website;
		set
		{
			AccountEntry.Website = value;
			
			OnPropertyChanged();
		}
	}
	
	public string? User
	{
		get => AccountEntry.User;
		set
		{
			AccountEntry.User = value;
			
			OnPropertyChanged();
		}
	}
	
	public string? Password
	{
		get => AccountEntry.Password;
		set
		{
			AccountEntry.Password = value;
			
			OnPropertyChanged();
		}
	}
}
