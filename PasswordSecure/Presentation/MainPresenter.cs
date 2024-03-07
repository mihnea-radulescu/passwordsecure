using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.DomainModel.CustomEventArgs;
using PasswordSecure.Presentation.ViewModels;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation;

public class MainPresenter
{
	static MainPresenter()
	{
		var encryptedFileTypes = new List<FilePickerFileType>
		{
			new FilePickerFileType("Encrypted files (*.encrypted)")
			{
				Patterns = new List<string>
				{
					"*.encrypted"
				}
			}
		};

		EncryptedFileCreateOptions = new FilePickerSaveOptions
		{
			DefaultExtension = ".encrypted",
			FileTypeChoices = encryptedFileTypes,
			ShowOverwritePrompt = true,
			Title = "Select New Encrypted Container File"
		};
		
		EncryptedFileOpenOptions = new FilePickerOpenOptions
		{
			AllowMultiple = false,
			FileTypeFilter = encryptedFileTypes,
			Title = "Select Encrypted Container File"
		};
	}
	
	public MainPresenter(
		IDataAccessService dataAccessService,
		IAssemblyVersionProvider assemblyVersionProvider,
		MainWindow mainView)
	{
		_dataAccessService = dataAccessService;
		_assemblyVersionProvider = assemblyVersionProvider;

		_mainView = mainView;
		_mainView.AccountEntryCollectionCreated += OnAccountEntryCollectionCreated;
		_mainView.AccountEntryCollectionLoaded += OnAccountEntryCollectionLoaded;
		_mainView.AccountEntryCollectionSaved += OnAccountEntryCollectionSaved;

		_mainView.CloseMenuClicked += OnCloseMenuClicked;
		_mainView.ExitMenuClicked += OnExitMenuClicked;
		_mainView.HelpMenuClicked += OnHelpMenuClicked;

		_accessParams = new AccessParams();
		_passwordViewModel = new PasswordViewModel(_accessParams);
	}

	#region Private
	
	private static readonly FilePickerSaveOptions EncryptedFileCreateOptions;
	private static readonly FilePickerOpenOptions EncryptedFileOpenOptions;
	
	private readonly IDataAccessService _dataAccessService;
	private readonly IAssemblyVersionProvider _assemblyVersionProvider;

	private readonly MainWindow _mainView;

	private readonly AccessParams _accessParams;
	private readonly PasswordViewModel _passwordViewModel;
	
	private async void OnAccountEntryCollectionCreated(object? sender, EventArgs e)
	{
		ResetData(false);

		try
		{
			await CreateEncryptedContainer();
		}
		catch (Exception ex)
		{
			ResetData(false);

			await DisplayErrorMessage(ex);
		}
	}
	
	private async void OnAccountEntryCollectionLoaded(object? sender, EventArgs e)
	{
		ResetData(true);

		try
		{
			await LoadEncryptedContainer();
		}
		catch (Exception ex)
		{
			ResetData(false);

			await DisplayErrorMessage(ex);
		}
	}

	private async void OnAccountEntryCollectionSaved(
		object? sender, AccountEntryCollectionEventArgs e)
	{
		try
		{
			SaveEncryptedContainer(e);
		}
		catch (Exception ex)
		{
			await DisplayErrorMessage(ex);
		}
	}

	private void OnCloseMenuClicked(object? sender, EventArgs e)
	{
		ResetData(false);
	}
	
	private void OnExitMenuClicked(object? sender, EventArgs e)
	{
		_mainView.Close();
	}
	
	private async void OnHelpMenuClicked(object? sender, EventArgs e)
	{
		await DisplayHelpMessage();
	}
	
	private async Task CreateEncryptedContainer()
	{
		var encryptedFile = await _mainView.StorageProvider.SaveFilePickerAsync(
			EncryptedFileCreateOptions);
		
		if (encryptedFile is null)
		{
			return;
		}

		var setMasterPasswordWindow = new SetMasterPasswordWindow
		{
			DataContext = _passwordViewModel
		};
		await setMasterPasswordWindow.ShowDialog(_mainView);

		if (_accessParams.Password is null)
		{
			return;
		}

		_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
		_dataAccessService.SaveAccountEntries(_accessParams, []);
		_mainView.PopulateData([]);
	}
	
	private async Task LoadEncryptedContainer()
	{
		var encryptedFile = (await _mainView.StorageProvider.OpenFilePickerAsync(
			EncryptedFileOpenOptions)).SingleOrDefault();

		if (encryptedFile is null)
		{
			return;
		}

		var inputMasterPasswordWindow = new InputMasterPasswordWindow
		{
			DataContext = _passwordViewModel
		};
		await inputMasterPasswordWindow.ShowDialog(_mainView);
		
		if (_accessParams.Password is null)
		{
			return;
		}
		
		_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(_accessParams);
		_mainView.PopulateData(accountEntryCollection);
	}
	
	private void SaveEncryptedContainer(AccountEntryCollectionEventArgs e)
	{
		if (_accessParams.FilePath is not null && _accessParams.Password is not null)
		{
			var accountEntryCollection = e.AccountEntryCollection;
			
			_dataAccessService.SaveAccountEntries(_accessParams, accountEntryCollection);
		}
	}

	private async Task DisplayErrorMessage(Exception ex)
	{
		var errorMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"Error",
			ex.Message,
			ButtonEnum.Ok,
			Icon.Error,
			WindowStartupLocation.CenterOwner);

		await errorMessageBox.ShowWindowDialogAsync(_mainView);
	}
	
	private async Task DisplayHelpMessage()
	{
		var helpMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"About Password Secure",
			_assemblyVersionProvider.AssemblyVersionString,
			ButtonEnum.Ok,
			Icon.Info,
			WindowStartupLocation.CenterOwner);

		await helpMessageBox.ShowWindowDialogAsync(_mainView);
	}

	private void ResetData(bool shouldSaveBackup)
	{
		_accessParams.Password = null;
		_accessParams.FilePath = null;
		_accessParams.ShouldSaveBackup = shouldSaveBackup;
		
		_mainView.ClearData();
	}

	#endregion
}
