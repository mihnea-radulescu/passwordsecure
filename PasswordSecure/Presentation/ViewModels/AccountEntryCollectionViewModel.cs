using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryCollectionViewModel : ViewModelBase
{
	public AccountEntryCollectionViewModel(IList<AccountEntry> accountEntries)
	{
		var accountEntryViewModelsAsList = accountEntries
			.Select(anAccountEntry => new AccountEntryViewModel(anAccountEntry))
			.ToList();
		
		AccountEntryViewModels = new ObservableCollection<AccountEntryViewModel>(
			accountEntryViewModelsAsList);
	}

	public ObservableCollection<AccountEntryViewModel> AccountEntryViewModels { get; set; }

	public AccountEntryViewModel? SelectedAccountEntryViewModel
	{
		get => _selectedAccountEntryViewModel;
		set
		{
			_selectedAccountEntryViewModel = value;
			OnPropertyChanged();
		}
	}
	
	#region Private

	private AccountEntryViewModel? _selectedAccountEntryViewModel;

	#endregion
}
