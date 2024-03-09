using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
		
		RegisterEventHandlers();

		AddNewAccountEntryCommand = GetAddNewAccountEntryCommand();
		DeleteAccountEntryCommand = GetDeleteAccountEntryCommand();
		SortAccountEntriesCommand = GetSortAccountEntriesCommand();

		EditPasswordCommand = GetEditPasswordCommand();
		CopyPasswordCommand = GetCopyPasswordCommand();
	}

	public ObservableCollection<AccountEntryViewModel> AccountEntryViewModels { get; set; }
	
	public bool HasChanged { get; set; }

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
			.OrderBy(anAccountEntry => anAccountEntry.Name)
			.ToList();

		var accountEntryCollection = new AccountEntryCollection(accountEntries);
		return accountEntryCollection;
	}

	public void UnregisterEventHandlers()
	{
		AccountEntryViewModels.CollectionChanged -= OnCollectionChanged;

		foreach (var anAccountEntryViewModel in AccountEntryViewModels)
		{
			anAccountEntryViewModel.PropertyChanged -= OnAccountEntryPropertyChanged;
		}
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
	
	private void RegisterEventHandlers()
	{
		AccountEntryViewModels.CollectionChanged += OnCollectionChanged;

		foreach (var anAccountEntryViewModel in AccountEntryViewModels)
		{
			anAccountEntryViewModel.PropertyChanged += OnAccountEntryPropertyChanged;
		}
	}

	private ICommand GetAddNewAccountEntryCommand()
		=> new RelayCommand(
			() =>
			{
				var newAccountEntry = new AccountEntry();
				var newAccountEntryViewModel = new AccountEntryViewModel(newAccountEntry);
				
				AccountEntryViewModels.Add(newAccountEntryViewModel);
				newAccountEntryViewModel.PropertyChanged += OnAccountEntryPropertyChanged;
				SelectedAccountEntryViewModel = newAccountEntryViewModel;
			});

	private ICommand GetDeleteAccountEntryCommand()
		=> new RelayCommand(
			() =>
			{
				if (SelectedAccountEntryViewModel is not null)
				{
					var existingAccountEntryViewModel = SelectedAccountEntryViewModel;
					existingAccountEntryViewModel.PropertyChanged -= OnAccountEntryPropertyChanged;
					AccountEntryViewModels.Remove(existingAccountEntryViewModel);
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
		UnregisterEventHandlers();
		
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
		
		RegisterEventHandlers();
	}

	private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		=> HasChanged = true;

	private void OnAccountEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
		=> HasChanged = true;

	#endregion
}
