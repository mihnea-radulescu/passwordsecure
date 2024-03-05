using System;
using System.Collections.ObjectModel;
using System.Linq;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Commands;

namespace PasswordSecure.Presentation.ViewModels;

public class AccountEntryCollectionViewModel : ViewModelBase
{
	public AccountEntryCollectionViewModel(
		IDataAccessService dataAccessService,
		AccountEntryCollection accountEntries)
	{
		_dataAccessService = dataAccessService;
		
		var accountEntryViewModelsAsList = accountEntries
			.Select(anAccountEntry => new AccountEntryViewModel(anAccountEntry))
			.ToList();

		foreach (var anAccountEntryViewModel in accountEntryViewModelsAsList)
		{
			anAccountEntryViewModel.NameChanged += OnNameChanged;
		}
		
		AccountEntryViewModels = new ObservableCollection<AccountEntryViewModel>(
			accountEntryViewModelsAsList);

		AddNewAccountEntryCommand = GetAddNewAccountEntryCommand();
		DeleteAccountEntryCommand = GetDeleteAccountEntryCommand();
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
	
	public RelayCommand AddNewAccountEntryCommand { get; }
	public RelayCommand DeleteAccountEntryCommand { get; }
	
	#region Private

	private readonly IDataAccessService _dataAccessService;
	
	private AccountEntryViewModel? _selectedAccountEntryViewModel;

	private RelayCommand GetAddNewAccountEntryCommand()
		=> new RelayCommand(
			arg =>
			{
				var newAccountEntry = new AccountEntry();
				var newAccountEntryViewModel = new AccountEntryViewModel(newAccountEntry);
				
				AccountEntryViewModels.Add(newAccountEntryViewModel);
				SelectedAccountEntryViewModel = newAccountEntryViewModel;
				
				SortAccountEntryViewModels();
			});

	private RelayCommand GetDeleteAccountEntryCommand()
		=> new RelayCommand(
			arg =>
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
