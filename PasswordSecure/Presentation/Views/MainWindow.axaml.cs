using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PasswordSecure.DomainModel;
using PasswordSecure.DomainModel.CustomEventArgs;
using PasswordSecure.Presentation.ViewModels;

namespace PasswordSecure.Presentation.Views;

public partial class MainWindow : Window, IMainView
{
	public MainWindow()
	{
		InitializeComponent();
	}

	public event EventHandler? AccountEntryCollectionCreated;
	public event EventHandler? AccountEntryCollectionLoaded;
	
	public event EventHandler<AccountEntryCollectionEventArgs>? AccountEntryCollectionSaved;
	
	public event EventHandler? CloseMenuClicked;
	public event EventHandler? ExitMenuClicked;
	public event EventHandler? HelpMenuClicked;
	
	public void PopulateData(AccountEntryCollection accountEntries)
	{
		_accountEntryCollectionViewModel = new AccountEntryCollectionViewModel(accountEntries);
		
		DataContext = _accountEntryCollectionViewModel;
	}
	
	#region Private

	private AccountEntryCollectionViewModel? _accountEntryCollectionViewModel;

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
