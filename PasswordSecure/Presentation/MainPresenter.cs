using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PasswordSecure.Application.Providers;
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
		
		_mainWindow.NewMenuClicked += OnNewMenuClicked;
		_mainWindow.OpenMenuClicked += OnOpenMenuClicked;
		_mainWindow.SaveMenuClicked += OnSaveMenuClicked;
		_mainWindow.CloseMenuClicked += OnCloseMenuClicked;
		_mainWindow.ExitMenuClicked += OnExitMenuClicked;
		_mainWindow.WindowClosing += OnWindowClosing;
		
		_mainWindow.HelpMenuClicked += OnHelpMenuClicked;

		_accessParams = new AccessParams();
	}

	#region Private

	private const int MinimumMasterPasswordLength = 8;
	
	private static readonly FilePickerSaveOptions EncryptedFileCreateOptions;
	private static readonly FilePickerOpenOptions EncryptedFileOpenOptions;
	
	private readonly IDataAccessService _dataAccessService;
	private readonly IAssemblyVersionProvider _assemblyVersionProvider;

	private readonly MainWindow _mainWindow;
	private readonly AccessParams _accessParams;

	private void OnVisualStateChanged(object? sender, EventArgs e)
		=> _mainWindow.EnableControls();
	
	private async void OnNewMenuClicked(object? sender, AccountEntryCollectionEventArgs e)
	{
		var shouldExitWithoutProcessing = await SuggestSaveChanges(e, ButtonEnum.YesNoCancel);
		if (shouldExitWithoutProcessing)
		{
			return;
		}
		
		ResetData();

		try
		{
			await CreateEncryptedContainer();
		}
		catch (Exception ex)
		{
			ResetData();

			await DisplayErrorMessage(ex);
		}
		finally
		{
			_mainWindow.EnableControls();
		}
	}
	
	private async void OnOpenMenuClicked(object? sender, AccountEntryCollectionEventArgs e)
	{
		var shouldExitWithoutProcessing = await SuggestSaveChanges(e, ButtonEnum.YesNoCancel);
		if (shouldExitWithoutProcessing)
		{
			return;
		}
		
		ResetData();

		try
		{
			await LoadEncryptedContainer();
		}
		catch (Exception ex)
		{
			ResetData();

			await DisplayErrorMessage(ex);
		}
		finally
		{
			_mainWindow.EnableControls();
		}
	}

	private async void OnSaveMenuClicked(object? sender, AccountEntryCollectionEventArgs e)
	{
		try
		{
			SaveEncryptedContainer(e.AccountEntryCollection);
		}
		catch (Exception ex)
		{
			await DisplayErrorMessage(ex);
		}
	}

	private async void OnCloseMenuClicked(object? sender, AccountEntryCollectionEventArgs e)
	{
		var shouldExitWithoutProcessing = await SuggestSaveChanges(e, ButtonEnum.YesNoCancel);
		if (shouldExitWithoutProcessing)
		{
			return;
		}
		
		ResetData();
		
		_mainWindow.EnableControls();
	}
	
	private async void OnExitMenuClicked(object? sender, AccountEntryCollectionEventArgs e)
	{
		var shouldExitWithoutProcessing = await SuggestSaveChanges(e, ButtonEnum.YesNoCancel);
		if (shouldExitWithoutProcessing)
		{
			return;
		}
		
		await _mainWindow.CloseWindow();
	}

	private async void OnWindowClosing(object? sender, AccountEntryCollectionEventArgs e)
	{
		await SuggestSaveChanges(e, ButtonEnum.YesNo);
		
		await _mainWindow.CloseWindow();
	}

	private async void OnHelpMenuClicked(object? sender, EventArgs e)
		=> await DisplayHelpMessage();
	
	private async Task<bool> SuggestSaveChanges(
		AccountEntryCollectionEventArgs e, ButtonEnum buttonEnum)
	{
		var shouldExitWithoutProcessing = false;
		
		if (e.HasChanged)
		{
			var buttonResult = await DisplayUnsavedChangesMessage(buttonEnum);

			if (buttonResult is ButtonResult.Yes)
			{
				SaveEncryptedContainer(e.AccountEntryCollection);
			}
			else if (buttonResult is ButtonResult.Cancel)
			{
				shouldExitWithoutProcessing = true;
			}
		}

		return shouldExitWithoutProcessing;
	}
	
	private async Task CreateEncryptedContainer()
	{
		var encryptedFile = await _mainWindow.StorageProvider.SaveFilePickerAsync(
			EncryptedFileCreateOptions);
		
		if (encryptedFile is null)
		{
			return;
		}

		var createMasterPasswordWindow = new CreateMasterPasswordWindow
		{
			MinimumPasswordLength = MinimumMasterPasswordLength
		};
		var createMasterPasswordViewModel = new CreateMasterPasswordViewModel(
			createMasterPasswordWindow, _accessParams);

		createMasterPasswordWindow.DataContext = createMasterPasswordViewModel;
		await createMasterPasswordWindow.ShowDialog(_mainWindow);

		if (_accessParams.Password is null)
		{
			return;
		}

		_accessParams.FilePath = encryptedFile.Path.LocalPath;
		_mainWindow.SetActiveFilePath(_accessParams.FilePath);

		var accountEntryCollection = new AccountEntryCollection();
		_dataAccessService.SaveAccountEntries(_accessParams, accountEntryCollection);
		_mainWindow.PopulateData(accountEntryCollection);
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
		_mainWindow.SetActiveFilePath(_accessParams.FilePath);
			
		var accountEntryCollection = _dataAccessService.ReadAccountEntries(_accessParams);
		_mainWindow.PopulateData(accountEntryCollection);
	}
	
	private void SaveEncryptedContainer(AccountEntryCollection? accountEntryCollection)
	{
		if (_accessParams.FilePath is not null &&
		    _accessParams.Password is not null &&
		    accountEntryCollection is not null)
		{
			_dataAccessService.SaveAccountEntries(_accessParams, accountEntryCollection);
			
			_mainWindow.ResetHasChangedFlag();
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
	
	private async Task<ButtonResult> DisplayUnsavedChangesMessage(ButtonEnum buttonEnum)
	{
		var unsavedChangesMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"Unsaved Changes",
			"There are unsaved changes. Would you like to save them?",
			buttonEnum,
			Icon.Question,
			WindowStartupLocation.CenterOwner);

		return await unsavedChangesMessageBox.ShowWindowDialogAsync(_mainWindow);
	}

	private void ResetData()
	{
		_accessParams.Password = null;
		_accessParams.FilePath = null;
		
		_mainWindow.ClearData();
	}

	#endregion
}
