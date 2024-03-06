using System;
using PasswordSecure.DomainModel;
using PasswordSecure.DomainModel.CustomEventArgs;

namespace PasswordSecure.Presentation.Views;

public interface IMainView
{
	event EventHandler? AccountEntryCollectionCreated;
	event EventHandler? AccountEntryCollectionLoaded;
	
	event EventHandler<AccountEntryCollectionEventArgs>? AccountEntryCollectionSaved;

	event EventHandler? ExitMenuClicked;
	event EventHandler? HelpMenuClicked;

	void ClearData();
	void PopulateData(AccountEntryCollection accountEntries);
	
	void Show();
	void Close();
}
