using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation.ViewModels;

public class SetMasterPasswordViewModel : ObservableObject, IPasswordViewModel
{
	public SetMasterPasswordViewModel(
		SetMasterPasswordWindow setMasterPasswordWindow,
		AccessParams accessParams)
	{
		_setMasterPasswordWindow = setMasterPasswordWindow;
		_accessParams = accessParams;
	}
	
	public string? Password
	{
		get => _accessParams.Password;
		set
		{
			SetProperty(
				_accessParams.Password,
				value,
				_accessParams,
				(accessParams, propertyValue) => accessParams.Password = propertyValue);
		}
	}
	
	#region Private

	private readonly SetMasterPasswordWindow _setMasterPasswordWindow;
	private readonly AccessParams _accessParams;
	
	#endregion
}
