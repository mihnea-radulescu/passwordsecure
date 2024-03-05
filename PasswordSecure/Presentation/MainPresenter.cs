using System;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation;

public class MainPresenter
{
	public MainPresenter(
		IDataAccessService dataAccessService,
		IMainView mainView)
	{
		_dataAccessService = dataAccessService;

		_mainView = mainView;
		_mainView.AccountEntryCollectionCreated += OnAccountEntryCollectionCreated;
	}

	#region Private
	
	private readonly IDataAccessService _dataAccessService;

	private readonly IMainView _mainView;

	private AccessParams? _accessParams;
	
	private void OnAccountEntryCollectionCreated(object? sender, EventArgs e)
	{
		var createMasterPasswordWindow = new CreateMasterPasswordWindow();
		createMasterPasswordWindow.Show((MainWindow)_mainView);
	}

	#endregion
}
