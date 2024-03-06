using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryCollectionViewModel : ObservableObject
{
	public AccountEntryCollectionViewModel(AccountEntryCollection accountEntries)
	{
		AccountEntryViewModels = FromAccountEntryCollection(accountEntries);

		AddNewAccountEntryCommand = GetAddNewAccountEntryCommand();
		DeleteAccountEntryCommand = GetDeleteAccountEntryCommand();
	}

	public ObservableCollection<AccountEntryViewModel> AccountEntryViewModels { get; set; }

	public AccountEntryViewModel? SelectedAccountEntryViewModel
	{
		get => _selectedAccountEntryViewModel;
		set => SetProperty(ref _selectedAccountEntryViewModel, value);
	}
	
	public ICommand AddNewAccountEntryCommand { get; }
	public ICommand DeleteAccountEntryCommand { get; }
	
	public AccountEntryCollection ToAccountEntryCollection()
	{
		var accountEntries = AccountEntryViewModels
			.Select(anAccountEntryViewModel => anAccountEntryViewModel.AccountEntry)
			.ToList();

		var accountEntryCollection = new AccountEntryCollection(accountEntries);
		return accountEntryCollection;
	}
	
	#region Private
	
	private AccountEntryViewModel? _selectedAccountEntryViewModel;
	
	private ObservableCollection<AccountEntryViewModel> FromAccountEntryCollection(
		AccountEntryCollection accountEntries)
	{
		var accountEntryViewModelsAsList = accountEntries
			.Select(anAccountEntry => new AccountEntryViewModel(anAccountEntry))
			.ToList();

		foreach (var anAccountEntryViewModel in accountEntryViewModelsAsList)
		{
			anAccountEntryViewModel.NameChanged += OnNameChanged;
		}
		
		var accountEntryViewModels = new ObservableCollection<AccountEntryViewModel>(
			accountEntryViewModelsAsList);

		return accountEntryViewModels;
	}

	private ICommand GetAddNewAccountEntryCommand()
		=> new RelayCommand(
			() =>
			{
				var newAccountEntry = new AccountEntry();
				var newAccountEntryViewModel = new AccountEntryViewModel(newAccountEntry);
				
				AccountEntryViewModels.Add(newAccountEntryViewModel);
				SelectedAccountEntryViewModel = newAccountEntryViewModel;
				
				SortAccountEntryViewModels();
			});

	private ICommand GetDeleteAccountEntryCommand()
		=> new RelayCommand(
			() =>
			{
				if (SelectedAccountEntryViewModel is not null)
				{
					AccountEntryViewModels.Remove(SelectedAccountEntryViewModel);
					
					SortAccountEntryViewModels();
				}
			});

	private void OnNameChanged(object? sender, EventArgs e)
		=> SortAccountEntryViewModels();
	
	private void SortAccountEntryViewModels()
	{
		var selectedAccountEntryViewModel = SelectedAccountEntryViewModel;
		
		var sortedAccountEntryViewModels = AccountEntryViewModels
			.OrderBy(anAccountEntryViewModel => anAccountEntryViewModel.Name)
			.ToList();

		foreach (var anAccountEntryViewModel in sortedAccountEntryViewModels)
		{
			AccountEntryViewModels.Remove(anAccountEntryViewModel);
			
			anAccountEntryViewModel.NameChanged -= OnNameChanged;
		}
		
		foreach (var anAccountEntryViewModel in sortedAccountEntryViewModels)
		{
			AccountEntryViewModels.Add(anAccountEntryViewModel);
			
			anAccountEntryViewModel.NameChanged += OnNameChanged;
		}

		SelectedAccountEntryViewModel = selectedAccountEntryViewModel;
	}

	#endregion
}
