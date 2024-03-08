using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryCollectionViewModel : ObservableObject
{
	public AccountEntryCollectionViewModel(
		MainWindow mainWindow,
		AccountEntryCollection accountEntries)
	{
		_mainWindow = mainWindow;
		
		AccountEntryViewModels = FromAccountEntryCollection(accountEntries);

		AddNewAccountEntryCommand = GetAddNewAccountEntryCommand();
		DeleteAccountEntryCommand = GetDeleteAccountEntryCommand();
		SortAccountEntriesCommand = GetSortAccountEntriesCommand();

		EditPasswordCommand = GetEditPasswordCommand();
		CopyPasswordCommand = GetCopyPasswordCommand();
	}

	public ObservableCollection<AccountEntryViewModel> AccountEntryViewModels { get; set; }

	public AccountEntryViewModel? SelectedAccountEntryViewModel
	{
		get => _selectedAccountEntryViewModel;
		set
		{
			SetProperty(ref _selectedAccountEntryViewModel, value);
			
			SelectedAccountEntryViewModelChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public event EventHandler? SelectedAccountEntryViewModelChanged;
	
	public ICommand AddNewAccountEntryCommand { get; }
	public ICommand DeleteAccountEntryCommand { get; }
	public ICommand SortAccountEntriesCommand { get; }
	
	public ICommand EditPasswordCommand { get; }
	public ICommand CopyPasswordCommand { get; }
	
	public AccountEntryCollection ToAccountEntryCollection()
	{
		var accountEntries = AccountEntryViewModels
			.Select(anAccountEntryViewModel => anAccountEntryViewModel.AccountEntry)
			.ToList();

		var accountEntryCollection = new AccountEntryCollection(accountEntries);
		return accountEntryCollection;
	}
	
	#region Private
	
	private readonly MainWindow _mainWindow;
	
	private AccountEntryViewModel? _selectedAccountEntryViewModel;
	
	private ObservableCollection<AccountEntryViewModel> FromAccountEntryCollection(
		AccountEntryCollection accountEntries)
	{
		var accountEntryViewModelsAsList = accountEntries
			.Select(anAccountEntry => new AccountEntryViewModel(anAccountEntry))
			.ToList();
		
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
	
	private ICommand GetSortAccountEntriesCommand()
		=> new RelayCommand(SortAccountEntryViewModels);
	
	private ICommand GetEditPasswordCommand()
		=> new RelayCommand(
			async() =>
			{
				if (SelectedAccountEntryViewModel is not null)
				{
					var setPasswordWindow = new SetPasswordWindow
					{
						DataContext = SelectedAccountEntryViewModel
					};
					await setPasswordWindow.ShowDialog(_mainWindow);
				}
			});
	
	private ICommand GetCopyPasswordCommand()
		=> new RelayCommand(
			async() =>
			{
				if (SelectedAccountEntryViewModel is not null)
				{
					var password = SelectedAccountEntryViewModel.Password;
					await _mainWindow.Clipboard!.SetTextAsync(password);
				}
			});
	
	private void SortAccountEntryViewModels()
	{
		var selectedAccountEntryViewModel = SelectedAccountEntryViewModel;
		
		var sortedAccountEntryViewModels = AccountEntryViewModels
			.OrderBy(anAccountEntryViewModel => anAccountEntryViewModel.Name)
			.ToList();

		foreach (var anAccountEntryViewModel in sortedAccountEntryViewModels)
		{
			AccountEntryViewModels.Remove(anAccountEntryViewModel);
		}
		
		foreach (var anAccountEntryViewModel in sortedAccountEntryViewModels)
		{
			AccountEntryViewModels.Add(anAccountEntryViewModel);
		}

		SelectedAccountEntryViewModel = selectedAccountEntryViewModel;
	}

	#endregion
}
