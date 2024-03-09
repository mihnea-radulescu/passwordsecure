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
	
	public event EventHandler<AccountEntryCollectionEventArgs>? NewMenuClicked;
	public event EventHandler<AccountEntryCollectionEventArgs>? OpenMenuClicked;
	public event EventHandler<AccountEntryCollectionEventArgs>? SaveMenuClicked;
	public event EventHandler<AccountEntryCollectionEventArgs>? CloseMenuClicked;
	public event EventHandler<AccountEntryCollectionEventArgs>? ExitMenuClicked;
	
	public event EventHandler? HelpMenuClicked;

	public void SetActiveFilePath(string? filePath)
		=> TextBlockActiveFilePath.Text = filePath;
	
	public void EnableControls()
	{
		var isFileLoaded = _accountEntryCollectionViewModel is not null;
		var canDataBeSorted = isFileLoaded &&
		                    _accountEntryCollectionViewModel!.AccountEntryViewModels.Count >= 2;
		
		MenuItemSave.IsEnabled = isFileLoaded;
		MenuItemClose.IsEnabled = isFileLoaded;
		
		var isAccountEntrySelected = false;
		var canCopyPassword = false;

		if (isFileLoaded)
		{
			var selectedAccountEntryViewModel = _accountEntryCollectionViewModel!.SelectedAccountEntryViewModel;
			
			isAccountEntrySelected = selectedAccountEntryViewModel is not null;
			canCopyPassword = isAccountEntrySelected &&
			                  !string.IsNullOrEmpty(selectedAccountEntryViewModel!.Password);
		}

		MenuItemDeleteAccountEntry.IsEnabled = isAccountEntrySelected;
		MenuItemSortAccountEntries.IsEnabled = canDataBeSorted;
		
		TextBoxSelectedName.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedUrl.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedUser.IsEnabled = isAccountEntrySelected;
		TextBoxSelectedNotes.IsEnabled = isAccountEntrySelected;
		ButtonSelectedEditPassword.IsEnabled = isAccountEntrySelected;
		ButtonSelectedCopyPassword.IsEnabled = canCopyPassword;
	}

	public void ClearData()
	{
		SetActiveFilePath(null);
		
		if (_accountEntryCollectionViewModel is not null)
		{
			_accountEntryCollectionViewModel.SelectedAccountEntryViewModelChanged -=
				OnSelectedAccountEntryViewModelChanged;
			
			_accountEntryCollectionViewModel.UnregisterEventHandlers();
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

	public void ResetHasChangedFlag()
	{
		if (_accountEntryCollectionViewModel is not null)
		{
			_accountEntryCollectionViewModel.HasChanged = false;
		}
	}
	
	#region Private

	private AccountEntryCollectionViewModel? _accountEntryCollectionViewModel;

	private void OnSelectedAccountEntryViewModelChanged(object? sender, EventArgs e)
		=> VisualStateChanged?.Invoke(this, EventArgs.Empty);
	
	private void OnMenuItemNewClick(object? sender, RoutedEventArgs e)
	{ 
		var accountEntryCollectionEventArgs = GetAccountEntryCollectionEventArgs();
		
		NewMenuClicked?.Invoke(this, accountEntryCollectionEventArgs);
	}
	
	private void OnMenuItemOpenClick(object? sender, RoutedEventArgs e)
	{ 
		var accountEntryCollectionEventArgs = GetAccountEntryCollectionEventArgs();
		
		OpenMenuClicked?.Invoke(this, accountEntryCollectionEventArgs);
	}
	
	private void OnMenuItemSaveClick(object? sender, RoutedEventArgs e)
	{
		var accountEntryCollectionEventArgs = GetAccountEntryCollectionEventArgs();
		
		SaveMenuClicked?.Invoke(this, accountEntryCollectionEventArgs);
	}
	
	private void OnMenuItemCloseClick(object? sender, RoutedEventArgs e)
	{ 
		var accountEntryCollectionEventArgs = GetAccountEntryCollectionEventArgs();
		
		CloseMenuClicked?.Invoke(this, accountEntryCollectionEventArgs);
	}
	
	private void OnMenuItemExitClick(object? sender, RoutedEventArgs e)
	{ 
		var accountEntryCollectionEventArgs = GetAccountEntryCollectionEventArgs();
		
		ExitMenuClicked?.Invoke(this, accountEntryCollectionEventArgs);
	}
	
	private void OnMenuItemHelpClick(object? sender, RoutedEventArgs e)
		=> HelpMenuClicked?.Invoke(this, EventArgs.Empty);
	
	private AccountEntryCollectionEventArgs GetAccountEntryCollectionEventArgs()
	{
		AccountEntryCollection? accountEntryCollection = null;
		var hasChanged = false;
		
		if (_accountEntryCollectionViewModel is not null)
		{
			accountEntryCollection = _accountEntryCollectionViewModel
				.ToAccountEntryCollection();

			hasChanged = _accountEntryCollectionViewModel.HasChanged;
		}
		
		var accountEntryCollectionEventArgs = new AccountEntryCollectionEventArgs(
			accountEntryCollection, hasChanged);

		return accountEntryCollectionEventArgs;
	}
	
	#endregion
}
