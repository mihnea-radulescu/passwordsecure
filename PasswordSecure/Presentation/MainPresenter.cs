using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
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
		MainWindow mainView)
	{
		_dataAccessService = dataAccessService;

		_mainView = mainView;
		_mainView.AccountEntryCollectionCreated += OnAccountEntryCollectionCreated;
		_mainView.AccountEntryCollectionLoaded += OnAccountEntryCollectionLoaded;
		_mainView.AccountEntryCollectionSaved += OnAccountEntryCollectionSaved;

		_mainView.CloseMenuClicked += OnCloseMenuClicked;

		_accessParams = new AccessParams();
		_accessParamsViewModel = new AccessParamsViewModel(_accessParams);
	}

	#region Private
	
	private static readonly FilePickerSaveOptions EncryptedFileCreateOptions;
	private static readonly FilePickerOpenOptions EncryptedFileOpenOptions;
	
	private readonly IDataAccessService _dataAccessService;

	private readonly MainWindow _mainView;

	private readonly AccessParams _accessParams;
	private readonly AccessParamsViewModel _accessParamsViewModel;
	
	private async void OnAccountEntryCollectionCreated(object? sender, EventArgs e)
	{
		ResetData(false);
		
		var createMasterPasswordWindow = new CreateMasterPasswordWindow
		{
			DataContext = _accessParamsViewModel
		};
		await createMasterPasswordWindow.ShowDialog(_mainView);
		
		var encryptedFile = await _mainView.StorageProvider.SaveFilePickerAsync(
			EncryptedFileCreateOptions);

		if (encryptedFile is not null && _accessParams.MasterPassword is not null)
		{
			_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
			_dataAccessService.SaveAccountEntries(_accessParams, []);
			_mainView.PopulateData([]);
		}
	}
	
	private async void OnAccountEntryCollectionLoaded(object? sender, EventArgs e)
	{
		ResetData(true);
		
		var inputMasterPasswordWindow = new InputMasterPasswordWindow
		{
			DataContext = _accessParamsViewModel
		};
		await inputMasterPasswordWindow.ShowDialog(_mainView);
		
		var encryptedFileList = await _mainView.StorageProvider.OpenFilePickerAsync(
			EncryptedFileOpenOptions);

		var encryptedFile = encryptedFileList.SingleOrDefault();

		if (encryptedFile is not null && _accessParams.MasterPassword is not null)
		{
			_accessParams.FilePath = encryptedFile.Path.LocalPath;
			
			var accountEntryCollection = _dataAccessService.ReadAccountEntries(_accessParams);
			_mainView.PopulateData(accountEntryCollection);
		}
	}
	
	private void OnAccountEntryCollectionSaved(
		object? sender, AccountEntryCollectionEventArgs e)
	{
		if (_accessParams.FilePath is not null && _accessParams.MasterPassword is not null)
		{
			var accountEntryCollection = e.AccountEntryCollection;
			
			_dataAccessService.SaveAccountEntries(_accessParams, accountEntryCollection);
		}
	}

	private void OnCloseMenuClicked(object? sender, EventArgs e)
	{
		ResetData(false);
	}

	private void ResetData(bool shouldSaveBackup)
	{
		_accessParams.MasterPassword = null;
		_accessParams.FilePath = null;
		_accessParams.ShouldSaveBackup = shouldSaveBackup;
		
		_mainView.ClearData();
	}

	#endregion
}
