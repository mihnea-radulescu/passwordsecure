using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PasswordSecure.DomainModel;
using PasswordSecure.DomainModel.CustomEventArgs;
using PasswordSecure.Presentation.ViewModels;

namespace PasswordSecure.Presentation.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	public event EventHandler? VisualStateChanged;
	
	public event EventHandler? AccountEntryCollectionCreated;
	public event EventHandler? AccountEntryCollectionLoaded;
	
	public event EventHandler<AccountEntryCollectionEventArgs>? AccountEntryCollectionSaved;
	
	public event EventHandler? CloseMenuClicked;
	public event EventHandler? ExitMenuClicked;
	public event EventHandler? HelpMenuClicked;
	
	public void EnableControls()
	{
		var isFileLoaded = _accountEntryCollectionViewModel is not null;
		var canDataBeSorted = isFileLoaded &&
		                    _accountEntryCollectionViewModel!.AccountEntryViewModels.Count >= 2;
		
		MenuItemSave.IsEnabled = isFileLoaded;
		MenuItemClose.IsEnabled = isFileLoaded;
		
		var isAccountEntrySelected = false;

		if (isFileLoaded)
		{
			isAccountEntrySelected = _accountEntryCollectionViewModel!.SelectedAccountEntryViewModel
				is not null;
		}

		MenuItemDeleteAccountEntry.IsEnabled = isAccountEntrySelected;
		MenuItemSortAccountEntries.IsEnabled = canDataBeSorted;
		
		TextBoxSelectedName.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedUrl.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedUser.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedNotes.IsEnabled = isAccountEntrySelected;
		ButtonSelectedEditPassword.IsEnabled = isAccountEntrySelected;
		ButtonSelectedCopyPassword.IsEnabled = isAccountEntrySelected;
	}

	public void ClearData()
	{
		if (_accountEntryCollectionViewModel is not null)
		{
			_accountEntryCollectionViewModel.SelectedAccountEntryViewModelChanged -=
				OnSelectedAccountEntryViewModelChanged;
		}
		
		_accountEntryCollectionViewModel = null;
		DataContext = _accountEntryCollectionViewModel;
	}
	
	public void PopulateData(AccountEntryCollection accountEntries)
	{
		_accountEntryCollectionViewModel = new AccountEntryCollectionViewModel(
			this, accountEntries);

		_accountEntryCollectionViewModel.SelectedAccountEntryViewModelChanged +=
			OnSelectedAccountEntryViewModelChanged;
		
		DataContext = _accountEntryCollectionViewModel;
	}
	
	#region Private

	private AccountEntryCollectionViewModel? _accountEntryCollectionViewModel;

	private void OnSelectedAccountEntryViewModelChanged(object? sender, EventArgs e)
		=> VisualStateChanged?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemNewClick(object? sender, RoutedEventArgs e)
		=> AccountEntryCollectionCreated?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemOpenClick(object? sender, RoutedEventArgs e)
		=> AccountEntryCollectionLoaded?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemSaveClick(object? sender, RoutedEventArgs e)
	{
		if (_accountEntryCollectionViewModel is not null)
		{
			var accountEntryCollection = _accountEntryCollectionViewModel
				.ToAccountEntryCollection();

			var accountEntryCollectionEventArgs = new AccountEntryCollectionEventArgs(
				accountEntryCollection);
			
			AccountEntryCollectionSaved?.Invoke(this, accountEntryCollectionEventArgs);
		}
	}
	
	private void OnMenuItemCloseClick(object? sender, RoutedEventArgs e)
		=> CloseMenuClicked?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemExitClick(object? sender, RoutedEventArgs e)
		=> ExitMenuClicked?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemHelpClick(object? sender, RoutedEventArgs e)
		=> HelpMenuClicked?.Invoke(this, EventArgs.Empty);
	
	#endregion
}
