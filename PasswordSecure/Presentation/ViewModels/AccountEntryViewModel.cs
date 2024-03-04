using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryViewModel : ViewModelBase
{
	public AccountEntryViewModel(AccountEntry accountEntry)
	{
		_accountEntry = accountEntry;
	}

	public string Name
	{
		get => _accountEntry.Name;
		set
		{
			_accountEntry.Name = value;
			OnPropertyChanged();
		}
	}

	public string? Website
	{
		get => _accountEntry.Website;
		set
		{
			_accountEntry.Website = value;
			OnPropertyChanged();
		}
	}
	
	public string? User
	{
		get => _accountEntry.User;
		set
		{
			_accountEntry.User = value;
			OnPropertyChanged();
		}
	}
	
	public string? Password
	{
		get => _accountEntry.Password;
		set
		{
			_accountEntry.Password = value;
			OnPropertyChanged();
		}
	}
	
	#region Private
	
	private readonly AccountEntry _accountEntry;
	
	#endregion
}
