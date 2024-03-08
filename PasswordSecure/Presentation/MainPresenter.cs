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
		MainWindow mainWindow)
	{
		_dataAccessService = dataAccessService;
		_assemblyVersionProvider = assemblyVersionProvider;

		_mainWindow = mainWindow;
		
		_mainWindow.VisualStateChanged += OnVisualStateChanged;
		
		_mainWindow.AccountEntryCollectionCreated += OnAccountEntryCollectionCreated;
		_mainWindow.AccountEntryCollectionLoaded += OnAccountEntryCollectionLoaded;
		_mainWindow.AccountEntryCollectionSaved += OnAccountEntryCollectionSaved;

		_mainWindow.CloseMenuClicked += OnCloseMenuClicked;
		_mainWindow.ExitMenuClicked += OnExitMenuClicked;
		_mainWindow.HelpMenuClicked += OnHelpMenuClicked;

		_accessParams = new AccessParams();
	}

	#region Private
	
	private static readonly FilePickerSaveOptions EncryptedFileCreateOptions;
	private static readonly FilePickerOpenOptions EncryptedFileOpenOptions;
	
	private readonly IDataAccessService _dataAccessService;
	private readonly IAssemblyVersionProvider _assemblyVersionProvider;

	private readonly MainWindow _mainWindow;
	private readonly AccessParams _accessParams;

	private void OnVisualStateChanged(object? sender, EventArgs e)
		=> _mainWindow.EnableControls();
	
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
		finally
		{
			_mainWindow.EnableControls();
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
		finally
		{
			_mainWindow.EnableControls();
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
		
		_mainWindow.EnableControls();
	}
	
	private async void OnExitMenuClicked(object? sender, EventArgs e)
	{
		await _mainWindow.Clipboard!.ClearAsync();
		
		_mainWindow.Close();
	}
	
	private async void OnHelpMenuClicked(object? sender, EventArgs e)
	{
		await DisplayHelpMessage();
	}
	
	private async Task CreateEncryptedContainer()
	{
		var encryptedFile = await _mainWindow.StorageProvider.SaveFilePickerAsync(
			EncryptedFileCreateOptions);
		
		if (encryptedFile is null)
		{
			return;
		}

		var setMasterPasswordWindow = new SetMasterPasswordWindow();
		var setMasterPasswordViewModel = new SetMasterPasswordViewModel(
			setMasterPasswordWindow, _accessParams);

		setMasterPasswordWindow.DataContext = setMasterPasswordViewModel;
		await setMasterPasswordWindow.ShowDialog(_mainWindow);

		if (_accessParams.Password is null)
		{
			return;
		}

		_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
		_dataAccessService.SaveAccountEntries(_accessParams, []);
		_mainWindow.PopulateData([]);
	}
	
	private async Task LoadEncryptedContainer()
	{
		var encryptedFile = (await _mainWindow.StorageProvider.OpenFilePickerAsync(
			EncryptedFileOpenOptions)).SingleOrDefault();

		if (encryptedFile is null)
		{
			return;
		}

		var inputMasterPasswordWindow = new InputMasterPasswordWindow();
		var inputMasterPasswordViewModel = new InputMasterPasswordViewModel(
			inputMasterPasswordWindow, _accessParams);

		inputMasterPasswordWindow.DataContext = inputMasterPasswordViewModel;
		await inputMasterPasswordWindow.ShowDialog(_mainWindow);
		
		if (_accessParams.Password is null)
		{
			return;
		}
		
		_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(_accessParams);
		_mainWindow.PopulateData(accountEntryCollection);
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

		await errorMessageBox.ShowWindowDialogAsync(_mainWindow);
	}
	
	private async Task DisplayHelpMessage()
	{
		var helpMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"About Password Secure",
			_assemblyVersionProvider.AssemblyVersionString,
			ButtonEnum.Ok,
			Icon.Info,
			WindowStartupLocation.CenterOwner);

		await helpMessageBox.ShowWindowDialogAsync(_mainWindow);
	}

	private void ResetData(bool shouldSaveBackup)
	{
		_accessParams.Password = null;
		_accessParams.FilePath = null;
		_accessParams.ShouldSaveBackup = shouldSaveBackup;
		
		_mainWindow.ClearData();
	}

	#endregion
}
